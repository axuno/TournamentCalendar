using YAXLib;

namespace TournamentCalendar.Collecting;

internal static class Storage
{
    internal const string FileBaseName = "Tourneys_";

    /// <summary>
    /// The name of the folder where tourney XML files are stored.
    /// </summary>
    public const string StorageFolderName = "Collect";

    /// <summary>
    /// Gets the file names from <see cref="StorageFolder"/> in descending order.
    /// </summary>
    /// <returns>The files with their full path in descending order.</returns>
    public static IEnumerable<string> GetFileNamesDescending()
    {
        EnsureStorageFolder();

        return Directory.GetFiles($"{StorageFolder}", $"{FileBaseName}*.xml")
            .Select(Path.GetFileName)
            .OrderByDescending(f => f)!;
    }

    /// <summary>
    /// Gets or sets the directory where tourney XML files are stored.
    /// </summary>
    public static string? StorageFolder { get; internal set; }

    /// <summary>
    /// Deserializes the <paramref name="filename"/> from the <see cref="StorageFolder"/> directory.
    /// If it can't be deserialized, a new instance of <see cref="CollectedTourneys"/> is returned.
    /// </summary>
    /// <param name="filename"></param>
    public static CollectedTourneys ReadTourneysFromFile(string filename)
    {
        var serializer = new YAXSerializer<CollectedTourneys>();
        var tourneys = serializer.DeserializeFromFile(Path.Combine(StorageFolder!, filename));
        return tourneys ?? new CollectedTourneys();
    }

    /// <summary>
    /// Serializes the <see cref="CollectedTourneys"/> to the <see cref="StorageFolder"/> directory.
    /// </summary>
    /// <param name="tourneys"></param>
    /// <param name="dateForFileName">The date to use as part of the file name.</param>
    /// <param name="overwriteExisting"></param>
    internal static void SaveTourneysToFile(CollectedTourneys tourneys, DateTime dateForFileName, bool overwriteExisting)
    {
        EnsureStorageFolder();

        var path = Path.Combine(StorageFolder!, $"{FileBaseName}{dateForFileName:yyyy-MM-dd}.xml");
        if (File.Exists(path) && !overwriteExisting) return;

        var serializer = new YAXSerializer<CollectedTourneys>();
        serializer.SerializeToFile(tourneys, path);
    }

    internal static DateTime GetLastCollectionDate(IEnumerable<DateTime> collectionDates, DateTime beforeThisDate)
    {
        return collectionDates.FirstOrDefault(d => d.Date <= beforeThisDate.Date); // default is DateTime.MinValue
    }

    internal static DateTime[] ExtractDatesFromFileNames(IEnumerable<string> fileNames)
    {
        return fileNames.Select(ExtractDateFromFileName).OrderByDescending(f => f).ToArray();
    }

    internal static DateTime ExtractDateFromFileName(string filename)
    {
        return DateTime.Parse(filename.Substring(FileBaseName.Length, "yyyy-MM-dd".Length));
    }

    internal static void RemoveOldImportFiles(int filesToKeep)
    {
        EnsureStorageFolder();

        var files = GetFileNamesDescending().ToList();
        foreach (var file in files.OrderByDescending(f => f).Skip(filesToKeep))
        {
            File.Delete(Path.Combine(StorageFolder!, file));
        }
    }

    private static void EnsureStorageFolder()
    {
        _ = StorageFolder ?? throw new InvalidOperationException($"{nameof(StorageFolder)} is not set");

        if (!Directory.Exists(StorageFolder))
            throw new InvalidOperationException($"{nameof(StorageFolder)} does not exist");
    }

    /// <summary>
    /// Merges the newly <see cref="CollectedTourneys"/> with any existing <see cref="CollectedTourneys"/>
    /// and serializes the result to the <see cref="StorageFolder"/> directory.
    /// </summary>
    /// <param name="currentTourneys">The list of <see cref="TourneyInfo"/>s to save.</param>
    /// <param name="dateForFileName">The date to use for the file name.</param>
    /// <param name="deltaToThisDate">The latest collection date to compare to.</param>
    /// <param name="overwriteExisting">Overwrite an existing file.</param>
    /// <param name="filesToKeep">The number of latest files to keep. Other will be deleted.</param>
    public static Task SaveTourneysToFile(List<TourneyInfo> currentTourneys, DateTime dateForFileName, DateTime deltaToThisDate, bool overwriteExisting, int filesToKeep)
    {
        var fileNames = Storage.GetFileNamesDescending().ToList();
        // Find the current latest file
        var fileIndex = fileNames.FindIndex(0, f => Storage.ExtractDateFromFileName(f).Date <= deltaToThisDate);

        // Read the file, if found
        var latestStoredTourneys = fileIndex != -1 ? Storage.ReadTourneysFromFile(fileNames[fileIndex]).Tourneys : new CollectedTourneys().Tourneys;

        // The lists in the tuple are not sorted
        var (sameTourneys, newTourneys, deletedTourneys)
            = Collectors.CompareTourneysByUrl(currentTourneys, latestStoredTourneys);

        var tourneysToSave = new List<TourneyInfo>(latestStoredTourneys.Count * 2);

        tourneysToSave.AddRange(newTourneys);
        tourneysToSave.AddRange(sameTourneys); // Keep existing records from existing file unchanged

        SaveTourneysToFile(
            new CollectedTourneys
            { Tourneys = tourneysToSave
                .OrderBy(t => t.ProviderId)
                .ThenByDescending(t => t.CollectedOn) // new tourneys come first
                .ToList() }, dateForFileName,
            overwriteExisting);

        RemoveOldImportFiles(filesToKeep);

        return Task.CompletedTask;
    }
}
