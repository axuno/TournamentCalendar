using System;
using YAXLib.Attributes;

namespace TournamentCalendar.Collectors;

public class Tourney
{
    [YAXAttributeForClass]
    public int ProviderId { get; set; }

    [YAXAttributeForClass]
    public string Url { get; set; } = string.Empty;

    [YAXAttributeForClass]
    public DateTime CollectedOn { get; set; }
}
