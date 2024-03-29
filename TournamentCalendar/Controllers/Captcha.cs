﻿using System.Drawing;
using Axuno.Web;

namespace TournamentCalendar.Controllers;

/// <summary>
/// Captcha Controller
/// </summary>
[Route(nameof(Captcha))]
public class Captcha : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        return await GetSvgContent();
    }

    private Task<ContentResult> GetSvgContent()
    {
        using var ci = new CaptchaSvgGenerator(null, 151, 51, Color.FromArgb(0x2A, 0x9D, 0x80),
            Color.FromArgb(0x2F, 0x2A, 0x9D, 0x80), Color.FromArgb(0x2A, 0x9D, 0x80));

        var result = ci.SetTextWithMathCalc(5).ToString(); // GenerateRandomString(5)
        HttpContext.Session.SetString(CaptchaSessionKeyName, result);

        // Change the response headers to output an un-cached image.
        HttpContext.Response.Clear();
        HttpContext.Response.Headers.Append("Expires", DateTime.UtcNow.Date.AddDays(-1).ToString("R"));
        HttpContext.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate");
        HttpContext.Response.Headers.Append("Pragma", "no-cache");

        HttpContext.Response.ContentType = "image/svg+xml";
        return Task.FromResult(Content(ci.Image));
    }

    private static string CaptchaSessionKeyName => CaptchaSvgGenerator.CaptchaSessionKeyName;
}
