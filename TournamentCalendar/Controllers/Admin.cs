using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentCalendar.Library.Authentication;

namespace TournamentCalendar.Controllers;

[Route(nameof(Admin))]
public class Admin : ControllerBase
{
    private readonly Microsoft.Extensions.Hosting.IHostApplicationLifetime _applicationLifetime;

    public Admin(Microsoft.Extensions.Hosting.IHostApplicationLifetime appLifetime)
    {
        _applicationLifetime = appLifetime;
    }

    [Authorize(Roles = Constants.RoleName.Admin)]
    [HttpGet(nameof(NetCoreInfo))]
    public IActionResult NetCoreInfo()
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet.exe",
                Arguments = "--info",
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };

        p.Start();
        var sdkFolder = @"c:\Program Files\dotnet\sdk\";
        var stdout = p.StandardOutput.ReadToEnd() + '\n' + $"Folder: {sdkFolder}" + '\n';
        p.WaitForExit();

        var f = Directory.GetDirectories(sdkFolder);
        f.ToList().ForEach(e => stdout += e + '\n');

        return Content(stdout);
    }

    [Authorize(Roles = Constants.RoleName.Admin)]
    [HttpGet(nameof(Restart))]
    public ActionResult Restart()
    {
        _applicationLifetime.StopApplication();
        return Content("Ok", "text/plain");
    }

    [HttpGet(nameof(HeartBeat))]
    public async Task<IActionResult> HeartBeat()
    {
        // called by cron job
        return await Task.FromResult(Content("Ok", "text/plain"));
    }
}
