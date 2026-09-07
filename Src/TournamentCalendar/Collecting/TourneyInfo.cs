using YAXLib.Attributes;
using YAXLib.Enums;

namespace TournamentCalendar.Collecting;

public class TourneyInfo
{
    [YAXAttributeForClass]
    public int ProviderId { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? Name { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public DateTime? Date { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? PostalCode { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? City { get; set; }

    [YAXAttributeForClass]
    [YAXDontSerializeIfNull]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? Link { get; set; }

    [YAXAttributeForClass]
    public DateTime CollectedOn { get; set; }
}
