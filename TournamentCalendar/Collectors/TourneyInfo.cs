using System;
using YAXLib.Attributes;

namespace TournamentCalendar.Collectors;

public class TourneyInfo
{
    [YAXAttributeForClass]
    public int ProviderId { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    public string? Name { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    public DateTime? Date { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    public string? PostalCode { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    public string? Link { get; set; }

    [YAXAttributeForClass]
    public DateTime CollectedOn { get; set; }
}
