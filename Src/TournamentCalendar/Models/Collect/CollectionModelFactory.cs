using TournamentCalendar.Collecting;
using TournamentCalendar.Data;

namespace TournamentCalendar.Models.Collect;

public class CollectionModelFactory
{
    private static IAppDb? _appDb;

    public static async Task<ListModel> CreateListModel(DateTime beforeThisDate, IAppDb appDb)
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

            await CheckForExistingLocalEntries(listModel);
        }
        catch (Exception e)
        {
            listModel = new ListModel { Errors = new[] { e }, CollectionDates = new[] { DateTime.MinValue }, LastCollectionDate = DateTime.MinValue };
        }

        return listModel;
    }

    private static async Task CheckForExistingLocalEntries(ListModel model)
    {
        if (model.NewTourneys == null || !model.NewTourneys.Any() || _appDb == null) return;

        var oldestEntryDate = model.NewTourneys.OrderBy(t => t.Date).First(t => t.Date != null).Date!.Value;

        model.ExistInCalendar = new List<string>();

        var calendarEntries = await _appDb.CalendarRepository.GetActiveOrDeletedTournaments(oldestEntryDate, CancellationToken.None);

        foreach (var tourney in model.NewTourneys)
        {
            var calendarSimilar = calendarEntries.Where(t => t.DateFrom.Date == tourney.Date?.Date && t.PostalCode == tourney.PostalCode).ToList();
            if (!calendarSimilar.Any() || tourney.Name == null) continue;

            var tourneyWords = tourney.Name.Split(new[] { ' ', '.', '-' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var calendarEntry in calendarSimilar)
            {
                var tournamentWords = calendarEntry.TournamentName.Split(new[] { ' ', '.', '-' },
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                var wordMatches = tournamentWords.Intersect(tourneyWords, StringComparer.InvariantCultureIgnoreCase).ToList();
                // matches currently switched off
                if (wordMatches.Count >= 0) model.ExistInCalendar.Add(tourney.Link!);
            }
        }
    }
}
