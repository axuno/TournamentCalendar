using Microsoft.Extensions.Primitives;
using TournamentCalendar.Data;
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
        // Cross Origin Request Sharing (CORS) - allow request from any domain:
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        _logger.LogInformation("Host: {RemoteIp}, Referrer: {Referrer}", GetRemoteIpAddress(xForwardedFor, remoteAddr), referrer);
        var model = new Models.Calendar.BrowseModel(_appDb);
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
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Response.ContentType = "text/javascript"; // IE < 9 does not support application/javascript
        _logger.LogInformation("Host: {Host}, Referrer: {Referrer}", GetRemoteIpAddress(xForwardedFor, remoteAddr), referrer);
        return PartialView(ViewName.ContentSynd.CalendarListPartialJs);
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return View("Test");
        /*var colOrig = ColorMath.FromHtmlColor("#006600"); // = darkgreen
        //colOrig = System.Drawing.Color.FromName("darkgreen");
        var foreCol = ColorMath.IsDarkColor(colOrig) ? "white" : "black";
        var colOrigString = "#" + colOrig.R.ToString("X2") + colOrig.G.ToString("X2") + colOrig.B.ToString("X2");
        var col1 = ColorMath.Lighten(colOrig, 0.35f);
        var colString1 = "#" + col1.R.ToString("X2") + col1.G.ToString("X2") + col1.B.ToString("X2");
        var foreCol1 = ColorMath.IsDarkColor(col1) ? "white" : "black";
        var col2 = ColorMath.Lighten(colOrig, 0.65f);
        var colString2 = "#" + col2.R.ToString("X2") + col2.G.ToString("X2") + col2.B.ToString("X2");
        var foreCol2 = ColorMath.IsDarkColor(col2) ? "white" : "black";
        return Content($"<html><body>{col2.GetBrightness()}<div style=\"background-color: {colOrigString};color:{foreCol}\">Orig</div><div style=\"background-color: {colString1};color:{foreCol1}\">abc</div><div style=\"background-color: {colString2};  color:{foreCol2}\">def</div></body></html>", "text/html");
        return View("Test");*/
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
