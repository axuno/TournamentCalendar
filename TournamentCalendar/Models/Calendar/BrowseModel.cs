﻿using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;

namespace TournamentCalendar.Models.Calendar;

public class BrowseModel
{
    private readonly IAppDb _appDb;
    private readonly EntityCollection<CalendarEntity> _tournaments = new();
    private readonly EntityCollection<SurfaceEntity> _surfaces = new();
    private readonly EntityCollection<PlayingAbilityEntity> _playingAbilities = new();

    public BrowseModel(IAppDb appDb)
    {
        _appDb = appDb;
    }

    public async Task Load(CancellationToken cancellationToken)
    {
        IsFiltered = false;
        await _appDb.CalendarRepository.GetAllActiveTournaments(_tournaments, cancellationToken);
        await _appDb.CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities, cancellationToken);
    }

    public async Task Load(string guid, CancellationToken cancellationToken)
    {
        IsFiltered = true;
        await _appDb.CalendarRepository.GetActiveTournaments(_tournaments, guid, cancellationToken);
        if (_tournaments.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(guid), guid, "No entry found.");
        }
        await _appDb.CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities, cancellationToken);
    }

    public async Task Load(long id, CancellationToken cancellationToken)
    {
        IsFiltered = true;
        await _appDb.CalendarRepository.GetActiveTournaments(_tournaments, id, cancellationToken);
        if (_tournaments.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id, "No entry found.");
        }
        await _appDb.CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities, cancellationToken);
    }

    public bool IsFiltered { get; private set; }

    public int Count => _tournaments.Count;

    public IEnumerable<CalendarEntityDisplayModel> DisplayModel =>
        _tournaments.Select(t => new CalendarEntityDisplayModel(t, _surfaces, _playingAbilities));
}
