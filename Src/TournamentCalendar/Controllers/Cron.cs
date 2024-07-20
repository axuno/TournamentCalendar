using System.Net;
using TournamentCalendar.Collecting;

namespace TournamentCalendar.Controllers;

/// <summary>
/// CronController
/// </summary>
[Route(nameof(Cron))]
public class Cron : ControllerBase
{
    private readonly ILogger<Cron> _logger;

    public Cron(ILogger<Cron> logger, IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public ContentResult Index()
    {
        return Content("<pre>This page is accessible only via cron job.</pre>");
    }

    #region *** Heartbeat ***

    private const string _internalPing = "internal-ping";

    /// <summary>
    /// Used by external cron job (every 1-5 minutes)
    /// in order to prevent application idle timeout shutdowns 
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(Heartbeat) + "/{id?}")]
    public ContentResult Heartbeat(string? id)
    {
        if (!ModelState.IsValid)
            id = "unknown id";

        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        return Content($"Completed - {DateTime.Now:G} {id ?? string.Empty}.", "text/plain", System.Text.Encoding.UTF8);
    }

    [HttpGet(nameof(Collect) + "/{key}")]
    public async Task<ContentResult> Collect([FromRoute] string key)
    {
        if (!ModelState.IsValid || key != Configuration.GetValue<string?>("CronApiKey"))
        {
            return new ContentResult { Content = "Wrong Cron API Key", StatusCode = 500 };
        }

        try
        {
            Storage.StorageFolder = Path.Combine(HostingEnvironment.WebRootPath, Storage.StorageFolderName);
            var tourneys = await Collectors.CollectTourneys();
            Storage.SaveTourneysToFile(tourneys, DateTime.UtcNow, false);
            _logger.LogInformation("Touneys collected and saved");
            return new ContentResult { Content = "Success" };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while collecting tourneys");
            return new ContentResult { Content = "Failure", StatusCode = 500 };
        }
    }

    /// <summary>
    /// Heartbeat url is used by application_end event in global.asax
    /// in order to initiate application restart by IIS.
    /// </summary>
    /// <returns>Returns the ping url to initiate application restart by IIS</returns>
    [NonAction]
    public static string GetHeartbeatUrl()
    {
        return "https://volleyball-turnier.de/cron/heartbeat/" + _internalPing;
    }

    #endregion
}
