using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;


namespace TournamentCalendar.Collectors;

public class ProviderA : ProviderBase
{
    public ProviderA(ILogger? logger = null) : base(logger)
    {
    }

    public override int ProviderId { get; init; } = 1;
    public override string ProviderName { get; init; } = "vobatu.de";
    public override Uri BaseAddress { get; set; } = new("https://www.vobatu.de");
    public override string StartPath { get; set; } = "/turniere/";
    protected internal override async Task<IElement?> GetTournamentSection(string page)
    {
        return (await ToHtmlDocument(page)).QuerySelector("#c4");
    }

    protected override string? GetPathToNextPage(IElement parentElement)
    {
        return parentElement.QuerySelectorAll("ul.pagination li.page-item.next > a").OfType<IHtmlAnchorElement>().Select(l => l.PathName).FirstOrDefault();
    }

    protected override List<string> ExtractLinks(IElement parentElement)
    {
        var links = parentElement.QuerySelectorAll("table tr td:nth-child(1) a").OfType<IHtmlAnchorElement>();
        return links.Select(l => BaseAddress.AbsoluteUri.ConcatPath(l.PathName.Trim())).ToList();
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
            _ = tournamentSection ??  throw new InvalidOperationException($"Tournament section not found in '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}'");
            links.AddRange(ExtractLinks(tournamentSection));
            nextPagePath = GetPathToNextPage(tournamentSection);
        }

        return links;
    }
}
