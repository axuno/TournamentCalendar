using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;

namespace TournamentCalendar.Collectors;

public class ProviderB : ProviderBase
{
    public ProviderB(ILogger? logger = null) : base(logger)
    {}

    public override int ProviderId { get; init; } = 2;
    public override string ProviderName { get; init; } = "volleyballer.de";
    public override Uri BaseAddress { get; set; } = new("https://www.volleyballer.de");
    public override string StartPath { get; set; } = "/turniere/volleyball-turniere-gesamtliste.php";
    protected internal override async Task<IElement?> GetTournamentSection(string page)
    {
        //return (await ToHtmlDocument(page)).QuerySelector("div.row:nth-child(2) > div:nth-child(1)");
        var doc = await ToHtmlDocument(page);
        return doc.QuerySelector("div.row > div.eight.columns");
    }

    protected override string? GetPathToNextPage(IElement parentElement)
    {
        return null;
    }

    protected override List<TourneyInfo> ExtractInfos(IElement parentElement)
    {
        var list = new List<TourneyInfo>();
        var now = DateTime.UtcNow;

        var h3Headers = parentElement.QuerySelectorAll("h3").OfType<IHtmlHeadingElement>().Where(h => h.QuerySelector("a[title='weiter']") != null);
        foreach (var h3Header in h3Headers)
        {
            var h3Link = h3Header.QuerySelector<IHtmlAnchorElement>("a[title='weiter']");
            var name = h3Link?.Text;
            var link = BaseAddress.AbsoluteUri.ConcatPath(h3Link?.PathName.Trim() ?? string.Empty);
            var infoBlock = h3Header.NextElementSibling?.QuerySelector("div.seven.column")?.TextContent.Trim();
            var blocks = infoBlock?.Split(" - ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            DateTime.TryParseExact((blocks?.FirstOrDefault())?[..10], new[] {"dd'.'MM'.'yyyy"},
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date);
            var postalCode = (blocks?.LastOrDefault())?[..5];
            
            list.Add(new TourneyInfo {
                ProviderId = ProviderId, Date = date, Name = name, PostalCode = postalCode, Link = link,
                CollectedOn = now
            });
        }

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
            var tournamentSection = await GetTournamentSection(html);
            _ = tournamentSection ?? throw new InvalidOperationException($"Tournament section not found in '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}'");
            _ = ExtractInfos(tournamentSection);
            infos.AddRange(ExtractInfos(tournamentSection));
            nextPagePath = GetPathToNextPage(tournamentSection);
        }

        return infos;
    }
}
