namespace TournamentCalendar.Collecting;

public class Collectors
{
    private static readonly IList<ICollector> CollectorList = new List<ICollector> { new CollectorA(), new CollectorB() };

    private Collectors()
    {
    }

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

    public static (IList<TourneyInfo> Same, IList<TourneyInfo> New, IList<TourneyInfo> Deleted) CompareTourneysByUrl(IList<TourneyInfo> latestTourneys, IList<TourneyInfo> olderTourneys)
    {
        var sameTourneys = olderTourneys.Where(older => latestTourneys.Any(latest => older.Link == latest.Link)).ToList();
        var deletedTourneys = olderTourneys.Where(older => latestTourneys.All(latest => latest.Link != older.Link)).ToList();
        var newTourneys = latestTourneys.Where(latest => olderTourneys.All(older => older.Link != latest.Link)).ToList();

        return (sameTourneys, newTourneys, deletedTourneys);
    }
}
