using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TournamentCalendar.Collectors;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;

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
            var files = Storage.GetFilesDescending().ToList();
            // Find the first file matching the data criteria
            var fileIndex = files.FindIndex(0, f => Storage.ExtractDateFromFileName(f).Date <= (beforeThisDate != DateTime.MinValue ? beforeThisDate.Date : DateTime.MaxValue));

            // Read the file, if found
            var latestTourneys = fileIndex != -1 ? Storage.ReadTourneysFromFile(files[fileIndex]).Tourneys : new CollectedTourneys().Tourneys;
            // Read the file before the latest
            var olderTourneys = beforeThisDate != DateTime.MinValue && fileIndex + 1 < files.Count ? Storage.ReadTourneysFromFile(files[fileIndex + 1]).Tourneys : new CollectedTourneys().Tourneys;

            (listModel.SameTourneys, listModel.NewTourneys, listModel.DeletedTourneys)
                = Storage.CompareTourneysByUrl(latestTourneys, olderTourneys);
            
            listModel.ImportDates = Storage.ExtractDatesFromFileNames(files);
            listModel.LastImportDate = Storage.GetLastCollectionDate(listModel.ImportDates, beforeThisDate);

            await CheckForExistingLocalEntries(listModel);
        }
        catch (Exception e)
        {
            listModel = new ListModel { Errors = new[] { e }, ImportDates = new[] { DateTime.MinValue }, LastImportDate = DateTime.MinValue };
        }

        return listModel;
    }

    private static async Task CheckForExistingLocalEntries(ListModel model)
    {
        if (model.NewTourneys == null || !model.NewTourneys.Any() || _appDb == null) return;

        model.ExistInCalendar = new List<string>();

        EntityCollection<CalendarEntity> calendarEntries = new();
        await _appDb.CalendarRepository.GetAllActiveTournaments(calendarEntries, CancellationToken.None);

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

                var matches = tournamentWords.Intersect(tourneyWords, StringComparer.InvariantCultureIgnoreCase);
                // text matches currently switch off:
                if (matches.Count() >= 0) model.ExistInCalendar.Add(tourney.Link!);
            }
        }
    }
}
