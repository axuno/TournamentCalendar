using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    [HttpGet]
    public ContentResult Heartbeat(string? id)
    {
        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        return Content($"Completed - {DateTime.Now:G} {id ?? string.Empty}.", "text/plain", System.Text.Encoding.UTF8);
    }

    [HttpGet("collect/{key}")]
    public async Task<ContentResult> Collect([FromRoute] string key)
    {
        if (key != Configuration.GetValue<string?>("CronApiKey"))
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
            _logger.LogError("Error while collecting tourneys {Exception}", e);
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
