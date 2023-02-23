using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TournamentCalendar.Data;
using TournamentCalendar.Models.Calendar;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalender.Data;
using MailMergeLib;

namespace TournamentCalendar.Models.Newsletter;

public class NewsletterModel
{
    private EntityCollection<CalendarEntity> _tournaments = new();
    private readonly EntityCollection<SurfaceEntity> _surfaces = new();
    private readonly EntityCollection<PlayingAbilityEntity> _playingAbilities = new();
    public async Task<NewsletterModel> InitializeAndLoad()
    {
        Newsletters = await SentNewsletterRepository.GetLastNewsletters();

        if (Newsletters.Count == 0)
        {
            await SaveFirstRecord();
            Newsletters = await SentNewsletterRepository.GetLastNewsletters();
        }
            
        LastSendDate = Newsletters.Last().StartedOn;
        await LoadCalendarEntitiesSinceLastSend();
        return this;
    }

    private static async Task SaveFirstRecord()
    {
        var now = DateTime.Now;
        var nl = new SentNewsletterEntity
        {
            StartedOn = now,
            CompletedOn = now,
            NumOfTournaments = 0,
            NumOfRecipients = 0
        };

        await GenericRepository.Save(nl, false);
    }

    private async Task LoadCalendarEntitiesSinceLastSend()
    {
        await CalendarRepository.GetAllActiveTournaments(_tournaments);
        await CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities);

        _tournaments = await CalendarRepository.GetActiveTournaments(LastSendDate);
    }

    public ICollection<SentNewsletterEntity> Newsletters { get; private set; } = new HashSet<SentNewsletterEntity>();

    public ICollection<CalendarEntityDisplayModel> CalendarDisplayModel
    {
        get
        {
            return _tournaments.Select(t => new CalendarEntityDisplayModel(t, _surfaces, _playingAbilities)).ToList();
        }
    }

    public DateTime LastSendDate { get; private set; }

    public async Task<ICollection<MailMergeAddress>> GetRecipients()
    {
        var subs = await InfoServiceRepository.GetActiveSubscribers();
        return subs.Select(s => new MailMergeAddress(MailAddressType.To, s.Email)).ToArray();
    }
}