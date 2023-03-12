using System;
using System.Collections.Generic;
using TournamentCalendar.Collectors;

namespace TournamentCalendar.Models.Collect;

public class ListModel
{
    public IList<ICollector> Collectors { get; } = TournamentCalendar.Collectors.Collectors.GetAll();

    public IList<TourneyInfo>? NewTourneys { get; internal set; }

    public IList<TourneyInfo>? DeletedTourneys { get; internal set; }

    public IList<TourneyInfo>? SameTourneys { get; internal set; }

    public IList<Exception> Errors { get; internal set; } = new List<Exception>();

    public DateTime[] ImportDates { get; internal set; } = Array.Empty<DateTime>();

    public DateTime LastImportDate { get; internal set; }

    public List<string> ExistInCalendar { get; internal set; } = new();
}
