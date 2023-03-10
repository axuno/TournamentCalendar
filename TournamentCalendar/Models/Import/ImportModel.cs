using System;
using System.Collections.Generic;
using TournamentCalendar.Collectors;

namespace TournamentCalendar.Models.TournamentImport;

public class ListModel
{
    public IList<IProvider> Providers { get; } = Collectors.Providers.GetAll();

    public IList<TourneyInfo>? NewTourneys { get; internal set; }

    public IList<TourneyInfo>? DeletedTourneys { get; internal set; }

    public IList<TourneyInfo>? SameTourneys { get; internal set; }

    public IList<Exception> Errors { get; internal set; } = new List<Exception>();

    public DateTime[] ImportDates { get; internal set; } = Array.Empty<DateTime>();

    public DateTime LastImportDate { get; internal set; }
}
