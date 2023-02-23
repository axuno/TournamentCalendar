using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TournamentCalendar.Models.Newsletter;

namespace TournamentCalendar.Controllers;

[Route("nl")]
public class Newsletter : ControllerBase
{
    [Route("show")]
    public async Task<IActionResult> Show()
    {
        var model = await new NewsletterModel().InitializeAndLoad();

        return View("Show", model);
    }

    [Route("send")]
    public async Task<IActionResult> Send()
    {
        return await Task.FromResult(Content("send"));
    }
}