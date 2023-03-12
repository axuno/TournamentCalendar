using System.Collections.Generic;
using System.Threading.Tasks;

namespace TournamentCalendar.Collectors;

public class Collectors
{
    private static readonly IList<ICollector> CollectorList = new List<ICollector>
        { new CollectorA(), new CollectorB() };

    public static IList<ICollector> GetAll()
    {
        return CollectorList;
    }

    public static async Task<CollectedTourneys> CollectTourneys()
    {
        var tourneys = new CollectedTourneys();

        foreach (var collector in GetAll())
        {
            var infos = await collector.GetAllTourneyInfos();
            tourneys.Tourneys.AddRange(infos);
        }

        return tourneys;
    }
}
