using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TournamentCalendar.Collectors;

public class Providers
{
    private static readonly IList<IProvider> ProviderList = new List<IProvider>
        { new ProviderA(), new ProviderB() };

    public static IList<IProvider> GetAll()
    {
        return ProviderList;
    }

    public static async Task<CollectedTourneys> CollectTourneys(DateTime dateWrittenToRecords)
    {
        var tourneys = new CollectedTourneys();

        foreach (var provider in GetAll())
        {
            var links = await provider.GetAllTourneyLinks();
            tourneys.Tourneys.AddRange(links.Select(href => new Tourney
                { ProviderId = provider.ProviderId, Url = href, CollectedOn = dateWrittenToRecords }));
        }

        return tourneys;
    }
}
