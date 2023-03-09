using System;
using System.Linq;
using System.Collections.Generic;
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

    protected override List<string> ExtractLinks(IElement parentElement)
    {
        var links = parentElement.QuerySelectorAll("a").OfType<IHtmlAnchorElement>();
        return links.Where(l => l.Text.ToLower() == "weiter").Select(l => BaseAddress.AbsoluteUri.ConcatPath(l.PathName.Trim())).ToList();
    }

    public override async Task<List<string>> GetAllTourneyLinks()
    {
        var links = new List<string>();

        var nextPagePath = StartPath;

        while (nextPagePath != null)
        {
            var html = await GetDocumentAsync(nextPagePath);
            _ = html ?? throw new InvalidOperationException($"Page '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}' not found");
            var tournamentSection = await GetTournamentSection(html);
            _ = tournamentSection ?? throw new InvalidOperationException($"Tournament section not found in '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}'");
            links.AddRange(ExtractLinks(tournamentSection));
            nextPagePath = GetPathToNextPage(tournamentSection);
        }

        return links;
    }
}
