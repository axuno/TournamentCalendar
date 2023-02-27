using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace TournamentCalendar.Models.TournamentImport;

public enum Provider
{
    Unknown,
    Volleyballer,
    Vobatu
}

public class Tournament
{
    [YAXAttributeForClass]
    public Provider Provider { get; set; }

    [YAXAttributeForClass]
    public string Url { get; set; } = string.Empty;

    [YAXAttributeForClass]
    public DateTime Date { get; set; }
}

public class ProviderTournaments
{
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    public List<Tournament> Tournaments { get; set; } = new();
}

public class ListModel
{
    public List<Tournament> AllTournaments { get; set; } = new();

    public Provider[] AllTournamentsProviders { get { return AllTournaments.GroupBy(at => at.Provider).Select(grp => grp.Key).ToArray(); } }

    public List<Tournament> NewTournaments { get; set; } = new();

    public Provider[] NewTournamentsProviders { get { return NewTournaments.GroupBy(at => at.Provider).Select(grp => grp.Key).ToArray(); } }

    public Dictionary<Provider, Exception> Errors { get; internal set; } = new();

    public DateTime[] ImportDates { get; internal set; } = Array.Empty<DateTime>();

    public DateTime LastImportDate { get; internal set; }
}


public class ImportModel
{
    const string FileBaseName = "ExtTournaments_";

    public ImportModel(string pathToImportFiles)
    {
        PathToImportFiles = pathToImportFiles;
    }

    public string PathToImportFiles { get; set; }

    public Dictionary<Provider, Exception> Errors { get; } = new();

    public async Task<ListModel> GetTournamentsAfterKeyDate(DateTime keyDate, Provider[] providers)
    {
        var listModel = new ListModel {Errors = Errors};
        try
        {
            var s = new YAXSerializer<ProviderTournaments>();

            var files = Directory.GetFiles($"{PathToImportFiles}", $"{FileBaseName}*.xml");
                    
            var latestFile = files.Where(f => string.Compare(Path.GetFileName(f), $"{FileBaseName}{keyDate:yyyy-MM-dd}.xml",
                StringComparison.Ordinal) <= 0).OrderByDescending(f => f).FirstOrDefault(); ;

            var currentImport = await DownloadFromProviders(providers);

            var latestImport = latestFile != null
                ? s.DeserializeFromFile(latestFile)?.Tournaments
                : new ProviderTournaments().Tournaments;

            var diff = currentImport.Where(latest => latestImport != null && latestImport.All(second => second.Url != latest.Url)).ToList();

            listModel.ImportDates = ExtractDatesFromFilenames(files);
            listModel.LastImportDate = keyDate == DateTime.MaxValue ? listModel.ImportDates.First() : keyDate;

            listModel.NewTournaments.AddRange(diff);

            if (latestImport != null) listModel.AllTournaments.AddRange(latestImport);
            listModel.AllTournaments.AddRange(diff);

            s.SerializeToFile(new ProviderTournaments {Tournaments = listModel.AllTournaments},
                Path.Combine(PathToImportFiles, $"{FileBaseName}{DateTime.Now:yyyy-MM-dd}.xml"));

            RemoveOldImportFiles(files);
        }
        catch (Exception e)
        {
            Errors.Add(Provider.Unknown, e);
            listModel = new ListModel { Errors = Errors, ImportDates = new[] { DateTime.MinValue }, LastImportDate = DateTime.MinValue };
        }

        return listModel;
    }

    private static DateTime[] ExtractDatesFromFilenames(string[] files)
    {
        return files.Select(file => DateTime.Parse(Path.GetFileName(file).Substring(FileBaseName.Length, 10))).OrderByDescending(f => f).ToArray();
    }

    private static void RemoveOldImportFiles(string[] files)
    {
        foreach (var file in files.OrderByDescending(f => f).Skip(10))
        {
            File.Delete(file);
        }
    }
		
    public async Task<List<Tournament>> DownloadFromProviders(Provider[] providers)
    {
        var now = DateTime.Now;
        var tournaments = new List<Tournament>();
        foreach (var provider in providers)
        {
            try
            {
                var hrefs = await DownloadLinksFromProvider(provider);
                tournaments.AddRange(hrefs.Select(href => new Tournament {Provider = provider, Url = href, Date = now}));
            }
            catch (Exception ex)
            {
                Errors.Add(provider, ex);
            }
        }
        return tournaments;
    }

    private static async Task<string[]> DownloadLinksFromProvider(Provider providerId)
    {
        var hrefs = new string[1];

        switch (providerId)
        {
            case Provider.Volleyballer:
                hrefs = await GetVolleyballerTournamentHrefList();
                break;
            case Provider.Vobatu:
                hrefs = await GetVobatuTournamentHrefList();
                break;
        }
        return hrefs;
    }

    private static async Task<string[]> GetVolleyballerTournamentHrefList()
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:56.0) Gecko/20100101 Firefox/56.0");

        var page = await httpClient.GetStringAsync("http://www.volleyballer.de/turniere/volleyball-turniere-gesamtliste.php");
        var parser = new HtmlParser();
        var document = parser.ParseDocument(page);
        var tournamentSection = document.QuerySelector("div.row:nth-child(2) > div:nth-child(1)");
        if (tournamentSection == null)
        {
            throw new Exception("Sektion für die Turniereinträge scheint auf der Seite nicht (mehr) zu existieren.");
        }
        var links = tournamentSection.QuerySelectorAll("a").OfType<IHtmlAnchorElement>();
        var hrefs = links.Where(l => l.Text.ToLower() == "weiter").Select(l => l.Href.Trim());
        return hrefs.ToArray();
    }

    private static async Task<string[]> GetVobatuTournamentHrefList()
    {
        var hrefs = new List<string>();
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:56.0) Gecko/20100101 Firefox/56.0");
        var parser = new HtmlParser();

        var pageNum = 1;
        var totalPages = 1;

        while (pageNum <= totalPages)
        {
            var page = await httpClient.GetStringAsync($"http://www.vobatu.de/volleyballturniere/?pagenum={pageNum}");
            var document = parser.ParseDocument(page);
            if (pageNum == 1)
            {
                totalPages =
                    int.Parse(
                        new System.Text.RegularExpressions.Regex(@"Seite 1\ von (\d)").Match(document.Body?.TextContent!).Groups[1].Value);
            }
            var tournamentSection = document.QuerySelector("#c31 > div:nth-child(1) > table:nth-child(4)");
            if (tournamentSection == null)
            {
                throw new Exception("Sektion für die Turniereinträge scheint auf der Seite nicht (mehr) zu existieren.");
            }
            var links = tournamentSection.QuerySelectorAll("a").OfType<IHtmlAnchorElement>();
            hrefs.AddRange(links.Where(l => l.Text.ToLower() == "info").Select(l => l.Href.Trim()));
            pageNum++;
        }

        return hrefs.ToArray();
    }

    private static void GetVolleyballerTournamentDetails()
    {
        const string dateFromCol = "Datum:", closingDateCol = "Meldeschluss:";

        var parser = new HtmlParser();

        var document = parser.ParseDocument(File.ReadAllText(@"c:\temp\volleyballer-eintrag.html"));
        var tournamentSection = document.QuerySelector("div.row:nth-child(2) > div:nth-child(1)");

        // var tournamentName = tournamentSection?.QuerySelector("h2:nth-child(3)")?.TextContent;


        var twoColRows = tournamentSection?.QuerySelectorAll("div.row");
        if (twoColRows == null) return;
            
        foreach (var twoColRow in twoColRows)
        {
            if (twoColRow.TextContent.Contains(dateFromCol))
            {
                var dates =
                    new System.Text.RegularExpressions.Regex(@"\d{2}\.\d{2}\.\d{4}").Matches(twoColRow
                        .TextContent);
                if (dates.Count > 0)
                {
                    var from = DateTime.Parse(dates[0].Value,
                        System.Globalization.CultureInfo.GetCultureInfo("de-de"));
                    var to = DateTime.Parse(dates[1].Value,
                        System.Globalization.CultureInfo.GetCultureInfo("de-de"));
                }
            }

            if (twoColRow.TextContent.Contains(closingDateCol))
            {
                var date = DateTime.Parse(new System.Text.RegularExpressions.Regex(@"\d{2}\.\d{2}\.\d{4}")
                    .Match(twoColRow.TextContent).Value);
            }
        }
    }
}