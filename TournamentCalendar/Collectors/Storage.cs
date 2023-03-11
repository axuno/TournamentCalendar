using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TournamentCalendar.Models.Collect;
using YAXLib;

namespace TournamentCalendar.Collectors;

internal static class Storage
{
    internal const string FileBaseName = "Tourneys_";

    /// <summary>
    /// Gets the files with their full path in descending order.
    /// </summary>
    /// <returns>The files with their full path in descending order.</returns>
    public static IEnumerable<string> GetFilesDescending()
    {
        return Directory.GetFiles($"{StorageFolder}", $"{FileBaseName}*.xml")
            .OrderByDescending(f => f);
    }

    /// <summary>
    /// Gets or sets the directory where tourney XML files are stored.
    /// </summary>
    public static string StorageFolder { get; internal set; } = string.Empty;

    public static CollectedTourneys ReadTourneysFromFile(string pathToFile)
    {
        var serializer = new YAXSerializer<CollectedTourneys>();
        var tourneys = serializer.DeserializeFromFile(pathToFile);
        return tourneys ?? new CollectedTourneys();
    }

    public static void SaveTourneysToFile(CollectedTourneys tourneys, bool overwriteExisting)
    {
        var filename = Path.Combine(StorageFolder, $"{FileBaseName}{DateTime.Now:yyyy-MM-dd}.xml");
        if (File.Exists(filename) && !overwriteExisting) return;

        var serializer = new YAXSerializer<CollectedTourneys>();
        serializer.SerializeToFile(tourneys, filename);
    }

    public static async Task CollectAndSaveTourneys(bool overwriteExisting)
    {
        var currentTourneys = (await Collectors.CollectTourneys()).Tourneys;

        SaveTourneysToFile(new CollectedTourneys { Tourneys = currentTourneys }, overwriteExisting);

        RemoveOldImportFiles(10);
    }

    public static (IList<TourneyInfo> Same, IList<TourneyInfo> New, IList<TourneyInfo> Deleted) CompareTourneysByUrl(IList<TourneyInfo> latestTourneys, IList<TourneyInfo> olderTourneys)
    {
        var sameTourneys = latestTourneys.Where(latest => olderTourneys.Any(older => older.Link == latest.Link)).ToList();
        var deletedTourneys = olderTourneys.Where(older => latestTourneys.All(latest => latest.Link != older.Link)).ToList();
        var newTourneys = latestTourneys.Where(latest => olderTourneys.All(older => older.Link != latest.Link)).ToList();

        return (sameTourneys, newTourneys, deletedTourneys);
    }

    internal static DateTime GetLastCollectionDate(IEnumerable<DateTime> collectionDates, DateTime beforeThisDate)
    {
        return collectionDates.FirstOrDefault(d => d.Date <= beforeThisDate.Date); // default is DateTime.MinValue
    }

    internal static DateTime[] ExtractDatesFromFileNames(IEnumerable<string> filesFullPath)
    {
        return filesFullPath.Select(ExtractDateFromFileName).OrderByDescending(f => f).ToArray();
    }

    internal static DateTime ExtractDateFromFileName(string fileFullPath)
    {
        return DateTime.Parse(Path.GetFileName(fileFullPath).Substring(FileBaseName.Length, "yyyy-MM-dd".Length));
    }

    internal static void RemoveOldImportFiles(int filesToKeep)
    {
        var files = GetFilesDescending().ToList();
        foreach (var file in files.OrderByDescending(f => f).Skip(filesToKeep))
        {
            File.Delete(file);
        }
    }
}
