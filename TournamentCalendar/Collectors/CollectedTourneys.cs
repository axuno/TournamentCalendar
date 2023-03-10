using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace TournamentCalendar.Collectors;

public class CollectedTourneys
{
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    public List<TourneyInfo> Tourneys { get; set; } = new();
}
