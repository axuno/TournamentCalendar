using System;
using System.Collections.Generic;
using System.Linq;
using TournamentCalendar.Collectors;

namespace TournamentCalendar.Models.TournamentImport;

public class ListModel
{
    public IList<IProvider> Providers { get; } = Collectors.Providers.GetAll();

    public List<Tourney> AllTournaments { get; set; } = new();

    public int[] AllTournamentsProviderIds =>
        AllTournaments.GroupBy(at => at.ProviderId).Select(grp => grp.Key).ToArray();

    public List<Tourney> NewTournaments { get; set; } = new();

    public IList<Exception> Errors { get; internal set; } = new List<Exception>();

    public DateTime[] ImportDates { get; internal set; } = Array.Empty<DateTime>();

    public DateTime LastImportDate { get; internal set; }

    public IProvider ProviderById(int providerId)
    {
        return Providers.First(p => p.ProviderId == providerId);
    }
}
