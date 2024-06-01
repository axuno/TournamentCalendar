using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Route("info")]
public class Info : ControllerBase
{
    [HttpGet("about-us")]
    public ActionResult LegalDetails()
    {
        ViewBag.TitleTagText = "Impressum";
        return View(ViewName.Info.LegalDetailsTournament);
    }

    [HttpGet("privacy")]
    public ActionResult PrivacyPolicy()
    {
        ViewBag.TitleTagText = "Datenschutzerklärung";
        return View(ViewName.Info.PrivacyPolicy);
    }
}
