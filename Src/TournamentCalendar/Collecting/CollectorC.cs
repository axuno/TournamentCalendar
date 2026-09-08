using System.Text.Json;
using AngleSharp.Dom;

namespace TournamentCalendar.Collecting;

/// <summary>
/// Collector for volleyballfreakportal.de.
/// This site is an Angular Universal (SSR) application. Rather than scraping the
/// server-rendered HTML (whose embedded Angular TransferState / ng-state JSON can be
/// stale relative to the live data), this collector calls the JSON search API
/// ("/api/tournaments/search") directly and deserializes the response into strongly
/// typed DTOs.
/// </summary>
public class CollectorC : CollectorBase
{
    private const string TournamentsSearchApiPath = "api/tournaments/search";

    public CollectorC(ILogger? logger = null) : base(logger)
    {
    }

    public override int ProviderId { get; init; } = 3;
    public override string ProviderName { get; init; } = "volleyballfreakportal.de";
    public override Uri BaseAddress { get; set; } = new("https://volleyballfreakportal.de");
    public override string StartPath { get; set; } = TournamentsSearchApiPath;

    /// <summary>
    /// <paramref name="page"/> is the raw JSON response body returned by the tournaments
    /// search API. It is wrapped in a &lt;script&gt; element so it can be handled as an
    /// <see cref="IElement"/>, consistent with the other collectors' abstractions.
    /// Returns <see langword="null"/> if <paramref name="page"/> is not valid JSON.
    /// </summary>
    protected internal override async Task<IElement?> GetTournamentSection(string page)
    {
        if (string.IsNullOrWhiteSpace(page)) return null;

        try
        {
            using var _ = JsonDocument.Parse(page);
        }
        catch (JsonException)
        {
            return null;
        }

        var document = await ToHtmlDocument($"<script>{page}</script>");
        return document.QuerySelector("script");
    }

    protected override string? GetPathToNextPage(IElement parentElement)
    {
        var result = GetSearchResult(parentElement);
        if (result is null) return null;

        // number is zero-based; if there are more pages, request the next one.
        var nextPageNumber = result.Number + 1;
        if (nextPageNumber >= result.TotalPages) return null;

        return $"{TournamentsSearchApiPath}?page={nextPageNumber}";
    }

    protected override List<TourneyInfo> ExtractInfos(IElement parentElement)
    {
        var list = new List<TourneyInfo>();
        var now = DateTime.UtcNow;

        var result = GetSearchResult(parentElement);
        if (result is null) return list;

        foreach (var tournament in result.Content)
        {
            list.Add(new TourneyInfo
            {
                ProviderId = ProviderId,
                Date = tournament.Date,
                Name = tournament.Name,
                PostalCode = tournament.PostalCode,
                City = tournament.City,
                Link = string.IsNullOrEmpty(tournament.Slug)
                    ? null
                    : BaseAddress.AbsoluteUri.ConcatPath($"portal/tournaments/{tournament.Slug}"),
                CollectedOn = now
            });
        }

        return list;
    }

    /// <summary>
    /// Parses the API response JSON (wrapped in a &lt;script&gt; element) and returns
    /// the deserialized search result.
    /// </summary>
    private static CollectorCSearchResult? GetSearchResult(IElement scriptElement)
    {
        var json = scriptElement.TextContent;
        if (string.IsNullOrWhiteSpace(json)) return null;

        return JsonSerializer.Deserialize<CollectorCSearchResult>(json);
    }

    public override async Task<List<TourneyInfo>> GetAllTourneyInfos()
    {
        var infos = new List<TourneyInfo>();

        var nextPagePath = StartPath;
        var visitedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        while (nextPagePath != null)
        {
            // Safeguard against an infinite loop in case the pagination query parameter
            // assumption (e.g. "?page=n") turns out to be incorrect for this site and the
            // same page keeps being returned.
            if (!visitedPaths.Add(nextPagePath))
            {
                Logger.LogWarning("Pagination loop detected for '{Path}', aborting collection for {Provider}.", nextPagePath, ProviderName);
                break;
            }

            var html = await GetDocumentAsync(nextPagePath);
            _ = html ?? throw new InvalidOperationException($"Page '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}' not found");
            var tournamentSection = await GetTournamentSection(html);
            _ = tournamentSection ?? throw new InvalidOperationException($"Tournament section not found in '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}'");

            infos.AddRange(ExtractInfos(tournamentSection));
            nextPagePath = GetPathToNextPage(tournamentSection);
        }

        return infos;
    }
}

