using TournamentCalendar.Data;
using TournamentCalendar.Models.Newsletter;
using TournamentCalendar.Services;

namespace TournamentCalendar.Controllers;

[Route("nl")]
public class Newsletter : ControllerBase
{
    private readonly IAppDb _appDb;
    private readonly UserLocation _userLocation;

    public Newsletter(IAppDb appDb, UserLocationService locationService)
    {
        _appDb = appDb;
        _userLocation = locationService.GetLocation();
    }

    [HttpGet("show")]
    public async Task<IActionResult> Show(CancellationToken cancellationToken)
    {
        var model = await new NewsletterModel(_appDb, _userLocation).InitializeAndLoad(cancellationToken);

        return View("Show", model);
    }

    [HttpGet("send")]
    public async Task<IActionResult> Send(CancellationToken cancellationToken)
    {
        return await Task.FromResult(Content("send"));
    }
}
