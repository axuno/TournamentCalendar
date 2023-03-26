using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;

namespace TournamentCalendar.Collecting;

public class CollectorA : CollectorBase
{
    public CollectorA(ILogger? logger = null) : base(logger)
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
    
    protected override List<TourneyInfo> ExtractInfos(IElement parentElement)
    {
        var list = new List<TourneyInfo>();
        var now = DateTime.UtcNow;

        var tableRows = parentElement.QuerySelectorAll("table tr").OfType<IHtmlTableRowElement>();
        foreach (var tableRow in tableRows)
        {
            var dateString = tableRow.QuerySelector<IHtmlAnchorElement>("td:nth-child(1) a")?.Text;
            if (string.IsNullOrEmpty(dateString)) continue;
            DateTime.TryParseExact(dateString, new[] {"dd'.'MM'.'yyyy"},
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date);
            var link = BaseAddress.AbsoluteUri.ConcatPath(tableRow.QuerySelector<IHtmlAnchorElement>("td:nth-child(1) a")?.PathName.Trim() ?? string.Empty);
            var postalCode = tableRow.QuerySelector<IHtmlAnchorElement>("td:nth-child(3) a")?.Text;
            var name = tableRow.QuerySelector<IHtmlAnchorElement>("td:nth-child(4) a")?.Text;
            
            list.Add(new TourneyInfo {
                ProviderId = ProviderId, Date = date, Name = name,
                PostalCode = postalCode, Link = link, CollectedOn = now
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
            _ = tournamentSection ??  throw new InvalidOperationException($"Tournament section not found in '{BaseAddress.AbsoluteUri.ConcatPath(nextPagePath)}'");
            infos.AddRange(ExtractInfos(tournamentSection));
            nextPagePath = GetPathToNextPage(tournamentSection);
        }

        return infos;
    }
}
