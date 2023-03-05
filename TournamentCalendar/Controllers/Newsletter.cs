﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TournamentCalendar.Models.Newsletter;

namespace TournamentCalendar.Controllers;

[Route("nl")]
public class Newsletter : ControllerBase
{
    [HttpGet("show")]
    public async Task<IActionResult> Show(CancellationToken cancellationToken)
    {
        var model = await new NewsletterModel().InitializeAndLoad(cancellationToken);

        return View("Show", model);
    }

    [HttpGet("send")]
    public async Task<IActionResult> Send(CancellationToken cancellationToken)
    {
        return await Task.FromResult(Content("send"));
    }
}
