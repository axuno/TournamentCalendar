namespace TournamentCalendar.Collecting;

public interface ICollector
{
    int ProviderId { get; init; }
    string ProviderName { get; init; }
    Uri BaseAddress { get; set; }
    string StartPath { get; set; }
    Task<List<TourneyInfo>> GetAllTourneyInfos();
}
