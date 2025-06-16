using TournamentCalendar.Data;
using TournamentCalendar.Services;
using TournamentCalendar.Views;
using TournamentCalendar.Models.GeoLocation;
using TournamentCalendar.Library;

namespace TournamentCalendar.Controllers;

/// <summary>
/// The GeoLocation controller is responsible for handling the user's location.
/// </summary>
[Route(nameof(GeoLocation))]
public class GeoLocation : ControllerBase
{
    private readonly UserLocationService _locationService;
    private readonly IAppDb _appDb;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoLocation"/> class.
    /// </summary>
    /// <param name="appDb"></param>
    /// <param name="locationService"></param>
    public GeoLocation(IAppDb appDb, UserLocationService locationService, IConfiguration configuration)
    {
        _appDb = appDb;
        _locationService = locationService;
        Configuration = configuration;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.TitleTagText = "Entfernungen anzeigen";
        var model = new EditModel();
        model.SetAppDb(_appDb);
        return View(ViewName.GeoLocation.Index, model);
    }

    /// <summary>
    /// Sets the location from a user's GUID.
    /// </summary>
    /// <param name="guid"></param>
    [HttpGet("location/{guid}")]
    public IActionResult Location(Guid guid)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Calendar.All), nameof(Controllers.Calendar), new[]{""});

        _locationService.SetFromUserGuid(guid);

        return RedirectToAction(nameof(Calendar.All), nameof(Controllers.Calendar));
    }

    [HttpPost(nameof(Location))]
    public async Task<IActionResult> Location([FromForm] EditModel model)
    {
        model.SetAppDb(_appDb);

        if (!ModelState.IsValid)
            return View(ViewName.GeoLocation.Index, model);

        var googleApi = new GoogleConfiguration();
        Configuration.Bind(nameof(GoogleConfiguration), googleApi);
        var userLocation = await model.TryGetLongitudeLatitude(googleApi);
        _locationService.SetGeoLocation(userLocation);

        return RedirectToAction(nameof(GeoLocation.Index), nameof(Controllers.GeoLocation));
    }

    [HttpGet("location/{latitude}/{longitude}")]
    public IActionResult Location(double latitude, double longitude)
    {
        if(!UserLocationService.IsValidLatitude(latitude))
            ModelState.AddModelError(nameof(latitude), "Latitude is invalid.");

        if (!UserLocationService.IsValidLongitude(longitude))
            ModelState.AddModelError(nameof(longitude), "Longitude is invalid.");

        if (!ModelState.IsValid)
            _locationService.ClearGeoLocation();

        _locationService.SetGeoLocation(latitude, longitude);

        return NoContent();
    }

    [HttpGet("clear")]
    public IActionResult ClearLocation()
    {
        _locationService.ClearGeoLocation();
        return RedirectToAction(nameof(GeoLocation.Index), nameof(Controllers.GeoLocation));
    }
}
