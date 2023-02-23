using Microsoft.AspNetCore.Mvc;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Route("info")]
public class Info : ControllerBase
{
    [Route("impressum")]
    public ActionResult LegalDetails()
    {
        ViewBag.TitleTagText = "Impressum";
        return View(ViewName.Info.LegalDetailsTournament);
    }

    [Route("datenschutz")]
    public ActionResult PrivacyPolicy()
    {
        ViewBag.TitleTagText = "Datenschutzerklärung";
        return View(ViewName.Info.PrivacyPolicy);
    }
}