using YAXLib.Attributes;
using YAXLib.Enums;

namespace TournamentCalendar.Collecting;

public class CollectedTourneys
{
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    public List<TourneyInfo> Tourneys { get; set; } = new();
}
