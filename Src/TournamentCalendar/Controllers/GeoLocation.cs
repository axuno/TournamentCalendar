using TournamentCalendar.Services;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

/// <summary>
/// The GeoLocation controller is responsible for handling the user's location.
/// </summary>
[Route(nameof(GeoLocation))]
public class GeoLocation : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserLocationService _locationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoLocation"/> class.
    /// </summary>
    /// <param name="locationService"></param>
    /// <param name="configuration"></param>
    public GeoLocation(UserLocationService locationService, IConfiguration configuration)
    {
        _locationService = locationService;
        _configuration = configuration;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.TitleTagText = "Entfernung anzeigen";
        return View(ViewName.GeoLocation.Index);
    }

    [HttpGet("location/{guid}")]
    public IActionResult Location(Guid guid)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Calendar.All), nameof(Controllers.Calendar));

        _locationService.SetFromUserGuid(guid);

        return RedirectToAction(nameof(Calendar.All), nameof(Controllers.Calendar));
    }

    [HttpGet("location/{latitude}/{longitude}")]
    public IActionResult Location(double latitude, double longitude)
    {
        if (!ModelState.IsValid)
            _locationService.ClearGeoLocation();

        _locationService.SetGeoLocation(latitude, longitude);

        return NoContent();
    }

    [HttpGet("clear")]
    public IActionResult ClearLocation()
    {
        _locationService.ClearGeoLocation();
        return NoContent();
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Calendar.All), nameof(Controllers.Calendar));
    }
}
