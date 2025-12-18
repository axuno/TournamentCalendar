using Microsoft.Extensions.Primitives;
using TournamentCalendar.Data;
using TournamentCalendar.Services;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Route("synd")]
public class ContentSynd : ControllerBase
{
    private readonly IAppDb _appDb;
    private readonly ILogger<ContentSynd> _logger;

    public ContentSynd(IAppDb appDb, ILogger<ContentSynd> logger)
    {
        _appDb = appDb;
        _logger = logger;
    }

    [HttpGet("calendar.html")]
    public async Task<IActionResult> CalendarList([FromHeader(Name = "Referrer")] string referrer, [FromHeader(Name = "X-Forwarded-For")] string[] xForwardedFor, [FromHeader(Name = "REMOTE_ADDR")] string[] remoteAddr, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            referrer = string.Empty;
            xForwardedFor = [];
            remoteAddr = [];
        }
        // Cross Origin Request Sharing (CORS) - allow request from any domain:
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        _logger.LogInformation("Host: {RemoteIp}, Referrer: {Referrer}", GetRemoteIpAddress(xForwardedFor, remoteAddr), referrer);
        var model = new Models.Calendar.BrowseModel(_appDb, new UserLocation(null, null));
        await model.Load(cancellationToken);
        return PartialView(ViewName.ContentSynd.CalendarListPartial, model);
    }

    [HttpGet("calendar.css")]
    public IActionResult CalendarListCss()
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Response.ContentType = "text/css";
        return PartialView(ViewName.ContentSynd.CalendarListPartialCss);
    }

    [HttpGet("calendar.js")]
    public IActionResult CalendarListJs([FromHeader(Name = "Referrer")] string referrer, [FromHeader(Name = "X-Forwarded-For")] string[] xForwardedFor, [FromHeader(Name = "REMOTE_ADDR")] string[] remoteAddr)
    {
        if (!ModelState.IsValid)
        {
            referrer = string.Empty;
            xForwardedFor = [];
            remoteAddr = [];
        }

        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Response.ContentType = "text/javascript"; // IE < 9 does not support application/javascript
        _logger.LogInformation("Host: {Host}, Referrer: {Referrer}", GetRemoteIpAddress(xForwardedFor, remoteAddr), referrer);
        return PartialView(ViewName.ContentSynd.CalendarListPartialJs);
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return View("Test");
    }

    private string GetRemoteIpAddress(string[] xForwardedFor, string[] remoteAddr)
    {
        //For this, app.UseForwardedHeaders(...) must be set in Startup
        if (!StringValues.IsNullOrEmpty(xForwardedFor))
        {
            return string.Join(',', xForwardedFor!);
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        if (!string.IsNullOrEmpty(ip) && ip.Length > 1)
        {
            return ip;
        }

        if (!StringValues.IsNullOrEmpty(remoteAddr))
        {
            return string.Join(',', remoteAddr!);
        }

        return "Remote IP unknown";
    }
}
