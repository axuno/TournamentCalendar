using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace TournamentCalendar.Collecting;

public partial class CollectorB : CollectorBase
{
    public CollectorB(ILogger? logger = null) : base(logger)
    {
    }

    public override int ProviderId { get; init; } = 2;
    public override string ProviderName { get; init; } = "volleyballer.de";
    public override Uri BaseAddress { get; set; } = new("https://www.volleyballer.de");
    public override string StartPath { get; set; } = "/turniere/";
    protected internal override async Task<IElement?> GetTournamentSection(string page)
    {
        return (await ToHtmlDocument(page)).QuerySelector("#site-main");
    }

    protected override string? GetPathToNextPage(IElement parentElement)
    {
        return parentElement.QuerySelector("div.pagination > a.next.page-numbers")?.GetAttribute("href");
    }

    [GeneratedRegex(@"<.*?>")]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"Von:\s*(\d{2}\.\d{2}\.\d{4})")]
    private static partial Regex FromDateRegex();

    [GeneratedRegex(@"Ort:.*?(\d{5})")]
    private static partial Regex PostalCodeRegex();

    protected override List<TourneyInfo> ExtractInfos(IElement parentElement)
    {
        var list = new List<TourneyInfo>();
        var now = DateTime.UtcNow;

        var name = parentElement.QuerySelector("header.detail-head h1")?.Text();
        var infoDetails = parentElement.QuerySelector("div.detail-grid div.detail-body p:has(strong)");

        string? dateString = null;
        string? postalCode = null;

        if (infoDetails is not null)
        {
            // Normalize <br /> to line breaks so lines don't run together
            var normalizedText = infoDetails.InnerHtml
                .Replace("<br />", "\n", StringComparison.OrdinalIgnoreCase)
                .Replace("<br/>", "\n", StringComparison.OrdinalIgnoreCase)
                .Replace("<br>", "\n", StringComparison.OrdinalIgnoreCase);

            // Strip remaining tags (e.g., <strong>...</strong>) to get plain text lines
            var plainText = HtmlTagRegex().Replace(normalizedText, string.Empty);

            var fromDateMatch = FromDateRegex().Match(plainText);
            if (fromDateMatch.Success) dateString = fromDateMatch.Groups[1].Value;

            var cityMatch = PostalCodeRegex().Match(plainText);
            if (cityMatch.Success) postalCode = cityMatch.Groups[1].Value;
        }

        DateTime.TryParseExact(dateString, ["dd'.'MM'.'yyyy"],
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date);

        var cityName = parentElement
            .QuerySelectorAll("aside.article-aside div.detail-facts div.row")
            .FirstOrDefault(row => row.QuerySelector("span.k")?.TextContent.Trim() == "Ort")
            ?.QuerySelector("span.v")?.TextContent.Trim();

        list.Add(new TourneyInfo
        {
            ProviderId = ProviderId, Date = date, Name = name,
            PostalCode = postalCode, City = cityName,
            Link = string.Empty, CollectedOn = now
        });

        return list;
    }

    public override async Task<List<TourneyInfo>> GetAllTourneyInfos()
    {
        var infos = new List<TourneyInfo>();

        var nextPagePath = StartPath;

        while (nextPagePath != null)
        {
            var html = await GetDocumentAsync(nextPagePath);
            _ = html ?? throw new InvalidOperationException($"Page '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}' not found");
            var currentPageTournamentsSection = await GetTournamentSection(html);
            _ = currentPageTournamentsSection ??  throw new InvalidOperationException($"Tournament section not found in '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}'");

            var links = currentPageTournamentsSection.QuerySelectorAll("a.entry-card")
                .Select(a => a.GetAttribute("href"))
                .Where(href => href is not null)
                .Select(href => href!)
                .ToList();

            foreach (var link in links)
            {
                var details = await GetDocumentAsync(link);
                _ = details ?? throw new InvalidOperationException($"Page '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}' not found");
                var tournamentSection = await GetTournamentSection(details);
                _ = tournamentSection ?? throw new InvalidOperationException($"Tournament section not found in '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}'");
                var singleInfo = ExtractInfos(tournamentSection).FirstOrDefault();

                if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri))
                    singleInfo?.Link = uri.IsAbsoluteUri ? uri.AbsoluteUri : BaseAddress.AbsoluteUri.ConcatPath(uri.OriginalString).Trim();

                if (singleInfo is not null) infos.Add(singleInfo);
            }
            
            nextPagePath = GetPathToNextPage(currentPageTournamentsSection);
        }

        return infos;
    }
}
