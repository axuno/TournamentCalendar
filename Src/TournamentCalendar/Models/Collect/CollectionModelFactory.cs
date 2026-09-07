using System.Text;
using TournamentCalendar.Collecting;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;

namespace TournamentCalendar.Models.Collect;

public class CollectionModelFactory
{
    private static IAppDb? _appDb;

    public static async Task<ListModel> CreateListModel(DateTime beforeThisDate, IAppDb appDb, CancellationToken cancellationToken)
    {
        _appDb = appDb;

        var listModel = new ListModel();
        try
        {
            var fileNames = Storage.GetFileNamesDescending().ToList();
            // Find the first file matching the data criteria
            var fileIndex = fileNames.FindIndex(0, f => Storage.ExtractDateFromFileName(f).Date <= (beforeThisDate != DateTime.MinValue ? beforeThisDate.Date : DateTime.MaxValue));

            // Read the file, if found
            var latestTourneys = fileIndex != -1 ? Storage.ReadTourneysFromFile(fileNames[fileIndex]).Tourneys : new CollectedTourneys().Tourneys;
            // Read the file before the latest
            var olderTourneys = beforeThisDate != DateTime.MinValue && fileIndex + 1 < fileNames.Count ? Storage.ReadTourneysFromFile(fileNames[fileIndex + 1]).Tourneys : new CollectedTourneys().Tourneys;

            (listModel.SameTourneys, listModel.NewTourneys, listModel.DeletedTourneys)
                = Collectors.CompareTourneysByUrl(latestTourneys, olderTourneys);
            
            listModel.CollectionDates = Storage.ExtractDatesFromFileNames(fileNames);
            listModel.LastCollectionDate = Storage.GetLastCollectionDate(listModel.CollectionDates, beforeThisDate);

            RemoveExpiredEntries(ref listModel, DateTime.Now.Date);
            await CheckForExistingLocalEntries(listModel, cancellationToken);
        }
        catch (Exception e)
        {
            listModel = new ListModel { Errors = [e], CollectionDates = [DateTime.MinValue], LastCollectionDate = DateTime.MinValue };
        }

        return listModel;
    }

    private static async Task CheckForExistingLocalEntries(ListModel model, CancellationToken cancellationToken)
    {
        if (model.NewTourneys == null || model.NewTourneys.Count == 0 || _appDb == null) return;

        var datedTourneys = model.NewTourneys.Where(t => t.Date != null).OrderBy(t => t.Date).ToList();
        var oldestEntryDate = datedTourneys.Min(t => t.Date!.Value);

        model.ExistInCalendar = [];

        var calendarEntries = await _appDb.CalendarRepository.GetActiveOrDeletedTournaments(oldestEntryDate, cancellationToken);

        foreach (var tourney in model.NewTourneys)
        {
            AddToCalendarMatchesIfSimilar(tourney, calendarEntries, model.ExistInCalendar);
        }
    }

    private static void AddToCalendarMatchesIfSimilar(TourneyInfo tourney, EntityCollection<CalendarEntity> calendarEntries, List<string> existInCalendar)
    {
        if (tourney.Date == null || tourney.Name == null) return;

        var calendarSimilar = calendarEntries
            .Where(t => t.DateFrom.Date == tourney.Date?.Date
                        && ((tourney.PostalCode != null && t.PostalCode == tourney.PostalCode) ||
                            (tourney.City != null && t.City == tourney.City))).ToList();
        if (calendarSimilar.Count == 0) return;

        var tourneyWords = NormalizeWords(tourney.Name);
        if (tourneyWords.Count == 0) return;

        if (calendarSimilar.Any(calendarEntry => IsSimilarName(tourneyWords, calendarEntry.TournamentName)))
            existInCalendar.Add(tourney.Link!);
    }

    private static bool IsSimilarName(List<string> tourneyWords, string tournamentName)
    {
        var tournamentWords = NormalizeWords(tournamentName);
        if (tournamentWords.Count == 0) return false;

        var wordMatches = tournamentWords.Intersect(tourneyWords, StringComparer.InvariantCultureIgnoreCase).Count();
        var minWordCount = Math.Min(tourneyWords.Count, tournamentWords.Count);
        var matchRatio = (double) wordMatches / minWordCount;

        // Require at least 2 matching words for longer names,
        // but allow a single-word match when both names are one word.
        var requiredMatches = minWordCount == 1 ? 1 : 2;

        return wordMatches >= requiredMatches && matchRatio >= 0.5;
    }

    private static List<string> NormalizeWords(string name)
    {
        var normalized = name
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
                        != UnicodeCategory.NonSpacingMark)
            .ToArray();

        var cleaned = new string(normalized).Normalize(NormalizationForm.FormC);

        return cleaned
            .Split([' ', '.', '-', ','], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    private static void RemoveExpiredEntries(ref ListModel model, DateTime referenceDate)
    {
        model.DeletedTourneys = model.DeletedTourneys?.Where(t => t.Date < referenceDate).ToList();
        model.NewTourneys = model.NewTourneys?.Where(t => t.Date >= referenceDate).ToList();
        model.SameTourneys = model.SameTourneys?.Where(t => t.Date >= referenceDate).ToList();
    }
}
