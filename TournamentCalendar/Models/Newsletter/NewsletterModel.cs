using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TournamentCalendar.Data;
using TournamentCalendar.Models.Calendar;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using MailMergeLib;
using System.Threading;

namespace TournamentCalendar.Models.Newsletter;

public class NewsletterModel
{
    private readonly IAppDb _appDb;
    private EntityCollection<CalendarEntity> _tournaments = new();
    private readonly EntityCollection<SurfaceEntity> _surfaces = new();
    private readonly EntityCollection<PlayingAbilityEntity> _playingAbilities = new();

    public NewsletterModel(IAppDb appDb)
    {
        _appDb = appDb;
    }

    public async Task<NewsletterModel> InitializeAndLoad(CancellationToken cancellationToken)
    {
        Newsletters = await _appDb.SentNewsletterRepository.GetLastNewsletters(cancellationToken);

        if (Newsletters.Count == 0)
        {
            await SaveFirstRecord(cancellationToken);
            Newsletters = await _appDb.SentNewsletterRepository.GetLastNewsletters(cancellationToken);
        }
            
        LastSendDate = Newsletters.Last().StartedOn;
        await LoadCalendarEntitiesSinceLastSend(cancellationToken);
        return this;
    }

    private async Task SaveFirstRecord(CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var nl = new SentNewsletterEntity
        {
            StartedOn = now,
            CompletedOn = now,
            NumOfTournaments = 0,
            NumOfRecipients = 0
        };

        await _appDb.GenericRepository.Save(nl, false, cancellationToken);
    }

    private async Task LoadCalendarEntitiesSinceLastSend(CancellationToken cancellationToken)
    {
        await _appDb.CalendarRepository.GetAllActiveTournaments(_tournaments, cancellationToken);
        await _appDb.CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities, cancellationToken);

        _tournaments = await _appDb.CalendarRepository.GetActiveTournaments(LastSendDate);
    }

    public ICollection<SentNewsletterEntity> Newsletters { get; private set; } = new HashSet<SentNewsletterEntity>();

    public ICollection<CalendarEntityDisplayModel> CalendarDisplayModel => _tournaments
        .Select(t => new CalendarEntityDisplayModel(t, _surfaces, _playingAbilities)).ToList();

    public DateTime LastSendDate { get; private set; }

    public async Task<ICollection<MailMergeAddress>> GetRecipients()
    {
        var subs = await _appDb.InfoServiceRepository.GetActiveSubscribers();
        return subs.Select(s => new MailMergeAddress(MailAddressType.To, s.Email)).ToArray();
    }
}
