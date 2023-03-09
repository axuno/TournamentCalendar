using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TournamentCalendar.Collectors;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Authorize(Roles = Library.Authentication.Constants.RoleName.Editor)]
[Route("import")]
public class Import : ControllerBase
{
    public Import(IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(hostingEnvironment, configuration)
    {}

    [HttpGet("anzeigen/{id?}")]
    public ActionResult Show(string id)
    {
        ViewBag.TitleTagText = "Andere Volleyball-Turnierkalender";

        Storage.StorageFolder = Path.Combine(HostingEnvironment.WebRootPath, @"Import");

        // Uses the latest stored tourneys and compares with current tourneys
        var beforeThisDate = DateTime.MaxValue;

        if (!string.IsNullOrEmpty(id))
        {
            DateTime.TryParseExact(id, new[] {"dd'.'MM'.'yyyy", "dd'.'MM'.'yy", "yyyy'-'MM'-'dd"},
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out beforeThisDate);
        }
            
        var listModel = Storage.CreateListModel(beforeThisDate);
        return View(ViewName.TournamentImport.Show, listModel);
    }
}
