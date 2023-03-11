using System;
using System.Linq;
using TournamentCalendar.Collectors;

namespace TournamentCalendar.Models.Collect;

public class CollectionModelFactory
{
    public static ListModel CreateListModel(DateTime beforeThisDate)
    {
        var listModel = new ListModel();
        try
        {
            var files = Storage.GetFilesDescending().ToList();
            // Find the first file matching the data criteria
            var fileIndex = files.FindIndex(0, f => Storage.ExtractDateFromFileName(f).Date <= beforeThisDate.Date);

            // Read the file, if found
            var latestTourneys = fileIndex != -1 ? Storage.ReadTourneysFromFile(files[fileIndex]).Tourneys : new CollectedTourneys().Tourneys;
            // Read the file before the latest
            var olderTourneys = fileIndex + 1 < files.Count ? Storage.ReadTourneysFromFile(files[fileIndex + 1]).Tourneys : new CollectedTourneys().Tourneys;

            (listModel.SameTourneys, listModel.NewTourneys, listModel.DeletedTourneys)
                = Storage.CompareTourneysByUrl(latestTourneys, olderTourneys);
            
            listModel.ImportDates = Storage.ExtractDatesFromFileNames(files);
            listModel.LastImportDate = Storage.GetLastCollectionDate(listModel.ImportDates, beforeThisDate);
        }
        catch (Exception e)
        {
            listModel = new ListModel { Errors = new[] { e }, ImportDates = new[] { DateTime.MinValue }, LastImportDate = DateTime.MinValue };
        }

        return listModel;
    }
}
