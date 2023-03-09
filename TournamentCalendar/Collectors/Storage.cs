using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TournamentCalendar.Models.TournamentImport;
using YAXLib;

namespace TournamentCalendar.Collectors;

public class Storage
{
    internal const string FileBaseName = "ExtTournaments_";

    public Storage(string storageFolder)
    {
        StorageFolder = storageFolder;
    }

    /// <summary>
    /// Gets the files with their full path in descending order.
    /// </summary>
    /// <returns>The files with their full path in descending order.</returns>
    public IEnumerable<string> GetFilesDescending()
    {
        return Directory.GetFiles($"{StorageFolder}", $"{FileBaseName}*.xml")
            .OrderByDescending(f => f);
    }

    /// <summary>
    /// Gets or sets the directory where tourney XML files are stored.
    /// </summary>
    public string StorageFolder { get; internal set; }

    public CollectedTourneys ReadLatestTourneyFile(DateTime beforeThisDate)
    {
        var latestFile = GetFilesDescending().FirstOrDefault(f => ExtractDateFromFileName(f).Date <= beforeThisDate.Date);
        if (latestFile == null) return new CollectedTourneys();

        var serializer = new YAXSerializer<CollectedTourneys>();
        var tourneys = serializer.DeserializeFromFile(Path.Combine(StorageFolder, latestFile));
        return tourneys ?? new CollectedTourneys();
    }

    public void SaveTourneysToFile(CollectedTourneys tourneys, bool overwriteExisting)
    {
        var filename = Path.Combine(StorageFolder, $"{FileBaseName}{DateTime.Now:yyyy-MM-dd}.xml");
        if (File.Exists(filename) && !overwriteExisting) return;

        var serializer = new YAXSerializer<CollectedTourneys>();
        serializer.SerializeToFile(tourneys, filename);
    }

    public async Task<ListModel> GetListModel(DateTime beforeThisDate)
    {
        var listModel = new ListModel();
        try
        {
            var files = GetFilesDescending().ToList();

            var currentTourneys = (await Providers.CollectTourneys(DateTime.Now)).Tourneys;

            var latestTourneysFromStorage = ReadLatestTourneyFile(beforeThisDate).Tourneys;

            var newComparedToStorage = currentTourneys.Where(latest => latestTourneysFromStorage.All(second => second.Url != latest.Url)).ToList();

            listModel.ImportDates = ExtractDatesFromFileNames(files);
            listModel.LastImportDate = GetLastCollectionDate(listModel.ImportDates, beforeThisDate);

            listModel.NewTournaments.AddRange(newComparedToStorage);

            listModel.AllTournaments.AddRange(latestTourneysFromStorage);
            listModel.AllTournaments.AddRange(newComparedToStorage);

            SaveTourneysToFile(new CollectedTourneys { Tourneys = listModel.AllTournaments }, false);

            RemoveOldImportFiles(files, 10);
        }
        catch (Exception e)
        {
            listModel = new ListModel { Errors = new[] {e}, ImportDates = new[] { DateTime.MinValue }, LastImportDate = DateTime.MinValue };
        }

        return listModel;
    }

    private static DateTime GetLastCollectionDate(IEnumerable<DateTime> collectionDates, DateTime beforeThisDate)
    {
        return beforeThisDate == DateTime.MaxValue
            ? collectionDates.First()
            : collectionDates.FirstOrDefault(d => d.Date <= beforeThisDate.Date); // default is DateTime.MinValue
    }

    private static DateTime[] ExtractDatesFromFileNames(IEnumerable<string> filesFullPath)
    {
        return filesFullPath.Select(ExtractDateFromFileName).OrderByDescending(f => f).ToArray();
    }

    private static DateTime ExtractDateFromFileName(string fileFullPath)
    {
        return DateTime.Parse(Path.GetFileName(fileFullPath).Substring(FileBaseName.Length, 10));
    }

    private static void RemoveOldImportFiles(IEnumerable<string> files, int filesToKeep)
    {
        foreach (var file in files.OrderByDescending(f => f).Skip(filesToKeep))
        {
            File.Delete(file);
        }
    }
}
