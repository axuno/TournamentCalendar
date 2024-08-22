using System.Web;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendar.Services;
using TournamentCalendarDAL.EntityClasses;

namespace TournamentCalendar.Models.Calendar;

public class CalendarEntityDisplayModel : CalendarEntity
{
    private readonly ICollection<SurfaceEntity> _surfaces;
    private readonly ICollection<PlayingAbilityEntity> _playingAbilities;
    private readonly UserLocation _userLocation;

    public CalendarEntityDisplayModel(IEntity2 t, UserLocation userLocation, ICollection<SurfaceEntity> surfaces, ICollection<PlayingAbilityEntity> playingAbilities)
    {
        // Make a deep copy
        base.Fields = t.Fields.Clone();
        _userLocation = userLocation;
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
            return "4er-Mixed";
        if (NumPlayersMale > 0 && NumPlayersFemale > 0 && NumPlayersMale + NumPlayersFemale == 2)
            return "2er-Mixed";
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

        return $"{tournamentType} - {NumPlayersMale + NumPlayersFemale} Spieler{(NumPlayersFemale > 0 && NumPlayersMale == 0 ? "innen" : "")}";
    }

    public string GetPlayingAbility()
    {
        string value;
        var from = _playingAbilities.First(pa => pa.Strength == PlayingAbilityFrom).Description;
        var to = _playingAbilities.First(pa => pa.Strength == PlayingAbilityTo).Description;

        if (PlayingAbilityTo == 0) // unlimited
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
        var completeAddress = (CountryId.Length > 0 ? CountryId + " " : string.Empty) +
                           (PostalCode.Length > 0 ? PostalCode + " " : string.Empty) +
                           (City.Length > 0 ? City + ", " : string.Empty) +
                           (Street.Length > 0 ? Street : string.Empty);

        if (maxChar.HasValue && completeAddress.Length > maxChar.Value)
        {
            return string.Concat(completeAddress.AsSpan(0, maxChar.Value - 3), "...".AsSpan());
        }

        return completeAddress;
    }

    public string GetVenueGoogleMapsLink()
    {
        if (!(Longitude.HasValue && Latitude.HasValue))
            return string.Empty;

        if (_userLocation.IsSet)
        {
            return string.Format("https://maps.google.de/maps?saddr={0},{1}&daddr={2},{3}",
                _userLocation.Latitude!.Value.ToString("###.########", CultureInfo.InvariantCulture),
                _userLocation.Longitude!.Value.ToString("###.########", CultureInfo.InvariantCulture),
                Latitude.Value.ToString("###.########", CultureInfo.InvariantCulture),
                Longitude.Value.ToString("###.########", CultureInfo.InvariantCulture));
        }

        return string.Format("https://maps.google.de?q={0},{1}",
            Latitude.Value.ToString("###.########", CultureInfo.InvariantCulture),
            Longitude.Value.ToString("###.########", CultureInfo.InvariantCulture));
    }

    public bool IsGeoSpatial()
    {
        return Longitude.HasValue && Latitude.HasValue && _userLocation.IsSet;
    }

    public string GetOrganizerShort(int maxChar)
    {
        if (Organizer.Length > maxChar)
            return string.Concat(Organizer.AsSpan(0, maxChar - 3), "...".AsSpan());

        return Organizer;
    }

    public string GetTournamentNameShort(int maxChar)
    {
        if (TournamentName.Length > maxChar)
            return string.Concat(TournamentName.AsSpan(0, maxChar - 3), "...".AsSpan());

        return TournamentName;
    }

    public int? GetDistanceToVenue()
    {
        if (!IsGeoSpatial())
            return null;

        var userLoc =
            new Axuno.Tools.GeoSpatial.Location(
                new Axuno.Tools.GeoSpatial.Latitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(_userLocation.Latitude!.Value)),
                new Axuno.Tools.GeoSpatial.Longitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(_userLocation.Longitude!.Value)));

        var venue =
            new Axuno.Tools.GeoSpatial.Location(
                new Axuno.Tools.GeoSpatial.Latitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(Latitude!.Value)),
                new Axuno.Tools.GeoSpatial.Longitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(Longitude!.Value)));

        var distance = (int) userLoc.Distance(venue)/1000;
        return distance;
    }

    public string GetContactAddress()
    {
        if (ContactAddress.Length > 0)
            return Axuno.Tools.String.StringHelper.NewLineToBreak(HttpUtility.HtmlEncode(ContactAddress));

        return string.Empty;
    }

    public string GetEntryFee()
    {
        var fee = decimal.Compare(EntryFee, (decimal) 0.01) > 0 ? string.Format("{0} Euro", EntryFee) : "keine";

        if (decimal.Compare(Bond, 0.01m) > 0)
            fee += $" plus {Bond} Euro Kaution";

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
