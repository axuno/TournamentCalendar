using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TournamentCalendar.Collectors;

public interface IProvider
{
    int ProviderId { get; init; }
    string ProviderName { get; init; }
    Uri BaseAddress { get; set; }
    string StartPath { get; set; }
    Task<List<TourneyInfo>> GetAllTourneyInfos();
}
