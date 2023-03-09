using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TournamentCalendar.Models.TournamentImport;
using YAXLib;

namespace TournamentCalendar.Collectors;

internal static class Storage
{
    internal const string FileBaseName = "ExtTournaments_";

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
        var tourneys = serializer.DeserializeFromFile(Path.Combine(StorageFolder, pathToFile));
        return tourneys ?? new CollectedTourneys();
    }

    public static void SaveTourneysToFile(CollectedTourneys tourneys, bool overwriteExisting)
    {
        var filename = Path.Combine(StorageFolder, $"{FileBaseName}{DateTime.Now:yyyy-MM-dd}.xml");
        if (File.Exists(filename) && !overwriteExisting) return;

        var serializer = new YAXSerializer<CollectedTourneys>();
        serializer.SerializeToFile(tourneys, filename);
    }

    public static async Task CollectAndSaveTourneys()
    {
        var currentTourneys = (await Providers.CollectTourneys(DateTime.Now)).Tourneys;

        SaveTourneysToFile(new CollectedTourneys { Tourneys = currentTourneys }, false);

        RemoveOldImportFiles(10);
    }

    public static (IList<Tourney> Same, IList<Tourney> New, IList<Tourney> Deleted) CompareTourneysByUrl(IList<Tourney> latestTourneys, IList<Tourney> olderTourneys)
    {
        var sameTourneys = latestTourneys.Where(latest => olderTourneys.Any(older => older.Url == latest.Url)).ToList();
        var deletedTourneys = olderTourneys.Where(older => latestTourneys.All(latest => latest.Url != older.Url)).ToList();
        var newTourneys = latestTourneys.Where(latest => olderTourneys.All(older => older.Url != latest.Url)).ToList();

        return (sameTourneys, newTourneys, deletedTourneys);
    }

    public static ListModel CreateListModel(DateTime beforeThisDate)
    {
        var listModel = new ListModel();
        try
        {
            var files = GetFilesDescending().ToList();
            // Find the first file matching the data criteria
            var fileIndex = files.FindIndex(0, f => ExtractDateFromFileName(f).Date <= beforeThisDate.Date);

            // Read the file, if found
            var latestTourneys = fileIndex != -1 ? ReadTourneysFromFile(files[fileIndex]).Tourneys : new CollectedTourneys().Tourneys;
            // Read the file before the latest
            var olderTourneys = fileIndex + 1 < files.Count ? ReadTourneysFromFile(files[fileIndex + 1]).Tourneys : new CollectedTourneys().Tourneys;

            (listModel.SameTourneys, listModel.NewTourneys, listModel.DeletedTourneys)
                = CompareTourneysByUrl(latestTourneys, olderTourneys);
            
            listModel.ImportDates = ExtractDatesFromFileNames(files);
            listModel.LastImportDate = GetLastCollectionDate(listModel.ImportDates, beforeThisDate);
        }
        catch (Exception e)
        {
            listModel = new ListModel { Errors = new[] {e}, ImportDates = new[] { DateTime.MinValue }, LastImportDate = DateTime.MinValue };
        }

        return listModel;
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
