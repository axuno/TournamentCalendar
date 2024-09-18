using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;

namespace TournamentCalendar.Services;

/// <summary>
/// The user location.
/// </summary>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
public record struct UserLocation(double? Latitude, double? Longitude)
{
    public bool IsSet => Latitude.HasValue
                         && UserLocationService.IsValidLatitude(Latitude.Value)
                         && Longitude.HasValue
                         && UserLocationService.IsValidLongitude(Longitude.Value);
}

/// <summary>
/// The user location service is responsible for handling the user's location.
/// </summary>
public class UserLocationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICookieService _cookieService;
    private readonly IAppDb _appDb;

    private const string LatLonFormat = "###.########";
    public const string UserLocationSessionName = nameof(UserLocationSessionName);

    /// <summary>
    /// Initializes a new instance of the <see cref="UserLocationService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="appDb"></param>
    /// <param name="cookieService"></param>
    public UserLocationService(IHttpContextAccessor httpContextAccessor, IAppDb appDb, ICookieService cookieService)
    {
        _httpContextAccessor = httpContextAccessor;
        _appDb = appDb;
        _cookieService = cookieService;
    }

    /// <summary>
    /// Sets the user's location.
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    public void SetGeoLocation(double latitude, double longitude)
    {
        SetGeoLocation(new UserLocation(latitude, longitude));
    }

    /// <summary>
    /// Sets the user's location if the <paramref name="userLocation"/> is set,
    /// otherwise clears the user's location.
    /// </summary>
    /// <param name="userLocation"></param>
    public void SetGeoLocation(UserLocation userLocation)
    {
        if(!userLocation.IsSet)
        {
            ClearGeoLocation();
            return;
        }

        var loc = Location2String(userLocation);
        _httpContextAccessor.HttpContext?.Session.SetString(UserLocationSessionName, loc);
        _cookieService.SetCookieValue(CookieService.LocationCookieName, loc, null);
    }

    /// <summary>
    /// Clears the user's location.
    /// </summary>
    public void ClearGeoLocation()
    {
        _httpContextAccessor.HttpContext?.Session.Remove(UserLocationSessionName);
        _cookieService.RemoveCookie(CookieService.LocationCookieName);
    }

    /// <summary>
    /// Sets the user's location from the user's GUID.
    /// </summary>
    /// <param name="userGuid"></param>
    public void SetFromUserGuid(Guid userGuid)
    {
        var infoService = new InfoServiceEntity();
        if (_appDb.InfoServiceRepository.GetRegistrationByGuid(infoService, userGuid.ToString("N")) &&
            infoService is { Latitude: not null, Longitude: not null })
        {
            SetGeoLocation(infoService.Latitude.Value, infoService.Longitude.Value);
            return;
        }

        ClearGeoLocation();
    }

    /// <summary>
    /// Gets the user's location from the session or the cookie.
    /// </summary>
    /// <returns></returns>
    public UserLocation GetLocation()
    {
        var userLocation = GetLocationFromSession();
        if (userLocation.IsSet)
        {
            return userLocation;
        }

        if (_cookieService.GetCookieValue(CookieService.LocationCookieName) is { } cookieLocation
            && TryString2Location(cookieLocation, out userLocation))
        {
            SetGeoLocation(userLocation);
            return userLocation;
        }

        return new UserLocation(null, null);
    }

    private UserLocation GetLocationFromSession()
    {
        if (_httpContextAccessor.HttpContext?.Session.GetString(UserLocationSessionName) is { } sessionLocation
            && TryString2Location(sessionLocation, out var userLocation))
        {
            return userLocation;
        }

        return new UserLocation(null, null);
    }

    private static bool TryString2Location(string location, out UserLocation userLocation)
    {
        var parts = location.Split('|');
        if (parts.Length == 2 &&
            double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var latitude) &&
            double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var longitude) &&
            IsValidLatitude(latitude) && IsValidLongitude(longitude))
        {
            userLocation = new UserLocation(latitude, longitude);
            return true;
        }

        userLocation = new UserLocation(null, null);
        return false;
    }

    private static string Location2String(UserLocation userLocation)
    {
        return userLocation.IsSet
            ? $"{userLocation.Latitude?.ToString(LatLonFormat, CultureInfo.InvariantCulture)}|{userLocation.Longitude?.ToString(LatLonFormat, CultureInfo.InvariantCulture)}"
            : string.Empty;
    }

    public static bool IsValidLatitude(double latDegrees)
    {
        return latDegrees is >= -90 and <= 90;
    }

    public static bool IsValidLongitude(double lonDegrees)
    {
        return lonDegrees is >= -180 and <= 180;
    }
}
