using System.Web;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;

namespace TournamentCalendar.Models.Calendar;

public class CalendarEntityDisplayModel : CalendarEntity
{
    private readonly EntityCollection<SurfaceEntity> _surfaces;
    private readonly EntityCollection<PlayingAbilityEntity> _playingAbilities;

    public CalendarEntityDisplayModel(IEntity2 t, EntityCollection<SurfaceEntity> surfaces, EntityCollection<PlayingAbilityEntity> playingAbilities)
    {
        // Make a deep copy
        base.Fields = t.Fields.Clone();
        _surfaces = surfaces;
        _playingAbilities = playingAbilities;
    }

    public string GetDates()
    {
        if (DateFrom.Date == DateTo.Date)
            return DateFrom.ToString("ddd, dd.MM.yyyy");

        return DateFrom.ToString("ddd, dd.MM.yyyy") + " bis " + DateTo.ToString("ddd, dd.MM.yyyy");
    }

    public string GetTimes()
    {
        var timeUnknown = new TimeSpan(0, 0, 0);
        if (DateFrom.TimeOfDay == timeUnknown && DateTo.TimeOfDay == timeUnknown)
            return "unbekannt";

        if (DateFrom.TimeOfDay != timeUnknown && DateTo.TimeOfDay == timeUnknown)
            return "ab " + DateFrom.ToString("HH:mm");

        return DateFrom.ToString("HH:mm") + " bis " + DateTo.ToString("HH:mm");
    }

    public string GetTournamentTypeShort()
    {
        if (NumPlayersFemale == 0)
            return "Herren";
        if (NumPlayersMale == 0)
            return "Damen";
        if (NumPlayersMale > 0 && NumPlayersFemale > 0)
            return "Mixed";

        return string.Empty;
    }

    public string GetTournamentType()
    {
        if (NumPlayersFemale == 0)
            return "Herren";
        if (NumPlayersMale == 0)
            return "Damen";
        if (NumPlayersMale > 0 && NumPlayersFemale > 0 && NumPlayersMale + NumPlayersFemale == 6)
            return "Mixed";
        if (NumPlayersMale > 0 && NumPlayersFemale > 0 && NumPlayersMale + NumPlayersFemale == 4)
            return "Quattro-Mixed";
        if (NumPlayersMale > 0 && NumPlayersFemale > 0 && NumPlayersMale + NumPlayersFemale == 2)
            return "Duo-Mixed";
        if (NumPlayersMale > 0 && NumPlayersFemale > 0)
            return "Mixed";

        return string.Empty;
    }


    public string GetTournamentTypeAndPlayers()
    {
        var tournamentType = GetTournamentType();
				
        if (NumPlayersMale > 0 && NumPlayersFemale > 0 && !IsMinPlayersFemale && !IsMinPlayersMale)
            return $"{tournamentType} - {NumPlayersMale} Herr{(NumPlayersMale > 1 ? "en" : "")} + {NumPlayersFemale} Dame{(NumPlayersFemale > 1 ? "n" : "")}";

        if (NumPlayersMale > 0 && NumPlayersFemale > 0 && IsMinPlayersFemale)
            return $"{tournamentType} - {NumPlayersMale + NumPlayersFemale} Spieler, mind. {NumPlayersFemale} Dame{(NumPlayersFemale > 1 ? "n" : "")}";

        if (NumPlayersMale > 0 && NumPlayersFemale > 0 && IsMinPlayersFemale)
            return $"{tournamentType} - {NumPlayersMale + NumPlayersFemale} Spieler, mind. {NumPlayersMale} Herr{(NumPlayersMale > 1 ? "en" : "")}";

        return String.Format("{0} - {1} Spieler{2}", tournamentType, NumPlayersMale + NumPlayersFemale,
            NumPlayersFemale > 0 && NumPlayersMale == 0 ? "innen" : "");
    }

    public string GetPlayingAbility()
    {
        string value;
        var from = _playingAbilities.First(pa => pa.Strength == PlayingAbilityFrom).Description;
        var to = _playingAbilities.First(pa => pa.Strength == PlayingAbilityTo).Description;

        if (PlayingAbilityTo == 0) // unbeschränkt
            to = string.Empty;

        if (from.Length > 0 && to.Length > 0)
        {
            value = from.Equals(to) ? from : string.Concat(from, " bis ", to);
        }
        else if (from.Length == 0 && to.Length > 0)
            value = "bis " + to;
        else if (from.Length > 0 && to.Length == 0)
            value = from;
        else
            value = _playingAbilities.First(pa => pa.Strength == 0).Description; ;

        return value;
    }

    public string GetVenueAddress(int? maxChar = null)
    {
        var completeAddr = (CountryId.Length > 0 ? CountryId + " " : string.Empty) +
                           (PostalCode.Length > 0 ? PostalCode + " " : string.Empty) +
                           (City.Length > 0 ? City + ", " : string.Empty) +
                           (Street.Length > 0 ? Street : string.Empty);

        if (maxChar.HasValue && completeAddr.Length > maxChar.Value)
        {
            return completeAddr.Substring(0, maxChar.Value - 3) + "...";
        }

        return completeAddr;
    }

    public string GetVenueGoogleMapsLink()
    {
        if (!(Longitude.HasValue && Latitude.HasValue))
            return string.Empty;

        return string.Format("http://maps.google.de?q={0},{1}",
            Latitude.Value.ToString("###.########", System.Globalization.CultureInfo.InvariantCulture),
            Longitude.Value.ToString("###.########", System.Globalization.CultureInfo.InvariantCulture));
    }

    public bool IsGeoSpatial()
    {
        return Longitude.HasValue && Latitude.HasValue;
    }

    public string GetOrganizerShort(int maxChar)
    {
        if (Organizer.Length > maxChar)
            return Organizer.Substring(0, maxChar - 3) + "...";

        return Organizer;
    }

    public string GetTournamentNameShort(int maxChar)
    {
        if (TournamentName.Length > maxChar)
            return TournamentName.Substring(0, maxChar - 3) + "...";

        return TournamentName;
    }

    public string GetVenueDistanceFromAugsburg()
    {
        if (!(Longitude.HasValue && Latitude.HasValue))
            return String.Empty;

        var augsburg =
            new Axuno.Tools.GeoSpatial.Location(
                new Axuno.Tools.GeoSpatial.Latitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(48.3666)),
                new Axuno.Tools.GeoSpatial.Longitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(10.894103)));

        var venue =
            new Axuno.Tools.GeoSpatial.Location(
                new Axuno.Tools.GeoSpatial.Latitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(Latitude.Value)),
                new Axuno.Tools.GeoSpatial.Longitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(Longitude.Value)));

        var distance = (int) augsburg.Distance(venue)/1000;
        return distance < 1
            ? string.Empty
            : string.Format("Entfernung nach Augsburg/Königsplatz ca. {0:0} km Luftlinie", distance);
    }

    public string GetContactAddr()
    {
        if (ContactAddress.Length > 0)
            return Axuno.Tools.String.StringHelper.NewLineToBreak(HttpUtility.HtmlEncode(ContactAddress));

        return string.Empty;
    }

    public string GetEntryFee()
    {
        var fee = decimal.Compare(EntryFee, (decimal) 0.01) > 0 ? string.Format("{0} Euro", EntryFee) : "keine";

        if (decimal.Compare(Bond, 0.01m) > 0)
            fee += string.Format(" plus {0} Euro Kaution", Bond);

        return fee;
    }

    public string GetInfo()
    {
        return Axuno.Tools.String.StringHelper.NewLineToBreak(HttpUtility.HtmlEncode(Info));
    }

 
    public string GetSurface()
    {
        return _surfaces.First(s => s.Id == Surface).Description;
    }
}