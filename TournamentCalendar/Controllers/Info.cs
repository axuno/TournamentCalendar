using Microsoft.AspNetCore.Mvc;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Route("info")]
public class Info : ControllerBase
{
    [HttpGet("impressum")]
    public ActionResult LegalDetails()
    {
        ViewBag.TitleTagText = "Impressum";
        return View(ViewName.Info.LegalDetailsTournament);
    }

    [HttpGet("datenschutz")]
    public ActionResult PrivacyPolicy()
    {
        ViewBag.TitleTagText = "Datenschutzerklärung";
        return View(ViewName.Info.PrivacyPolicy);
    }
}
