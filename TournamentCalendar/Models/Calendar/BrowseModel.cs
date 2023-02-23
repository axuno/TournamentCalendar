using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;

namespace TournamentCalendar.Models.Calendar;

public class BrowseModel
{
    private readonly EntityCollection<CalendarEntity> _tournaments = new();
    private readonly EntityCollection<SurfaceEntity> _surfaces = new();
    private readonly EntityCollection<PlayingAbilityEntity> _playingAbilities = new();

    public async Task Load()
    {
        IsFiltered = false;
        await CalendarRepository.GetAllActiveTournaments(_tournaments);
        await CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities);
    }

    public async Task Load(string guid)
    {
        IsFiltered = true;
        await CalendarRepository.GetActiveTournaments(_tournaments, guid);
        if (_tournaments.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(guid), guid, "No entry found.");
        }
        await CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities);
    }

    public async Task Load(long id)
    {
        IsFiltered = true;
        await CalendarRepository.GetActiveTournaments(_tournaments, id);
        if (_tournaments.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id, "No entry found.");
        }
        await CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities);
    }

    public bool IsFiltered { get; private set; }

    public int Count => _tournaments.Count;

    public IEnumerable<CalendarEntityDisplayModel> DisplayModel =>
        _tournaments.Select(t => new CalendarEntityDisplayModel(t, _surfaces, _playingAbilities));
}