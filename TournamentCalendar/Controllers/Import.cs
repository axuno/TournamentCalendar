using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TournamentCalendar.Models.TournamentImport;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Authorize(Roles = Library.Authentication.Constants.RoleName.Editor)]
[Route("import")]
public class Import : ControllerBase
{
    public Import(IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(hostingEnvironment, configuration)
    {}

    [HttpGet("anzeigen/{id?}")]
    public async Task<ActionResult> ShowAsync(string id)
    {
        ViewBag.TitleTagText = "Andere Volleyball-Turnierkalender";

        var model = new ImportModel(Path.Combine(HostingEnvironment.WebRootPath, @"Import"));
        var keyDate = DateTime.MaxValue;

        if (!string.IsNullOrEmpty(id))
        {
            DateTime.TryParseExact(id, new[] {"dd'.'MM'.'yyyy", "dd'.'MM'.'yy", "yyyy'-'MM'-'dd"},
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out keyDate);
        }
            
        var listModel = await model.GetTournamentsAfterKeyDate(keyDate, new[] { Provider.Volleyballer, Provider.Vobatu });
        return View(ViewName.TournamentImport.Show, listModel);
    }
}
