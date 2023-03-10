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

    public static async Task<CollectedTourneys> CollectTourneys()
    {
        var tourneys = new CollectedTourneys();

        foreach (var provider in GetAll())
        {
            var infos = await provider.GetAllTourneyInfos();
            tourneys.Tourneys.AddRange(infos);
        }

        return tourneys;
    }
}
