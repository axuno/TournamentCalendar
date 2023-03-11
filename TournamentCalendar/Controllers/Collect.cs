using System;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TournamentCalendar.Collectors;
using TournamentCalendar.Models.Collect;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Authorize(Roles = Library.Authentication.Constants.RoleName.Editor)]
[Route("import")]
public class Collect : ControllerBase
{
    public Collect(IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(hostingEnvironment, configuration)
    {}

    [HttpGet("anzeigen/{id?}")]
    public ActionResult Show(string id)
    {
        ViewBag.TitleTagText = "Andere Volleyball-Turnierkalender";

        Storage.StorageFolder = Path.Combine(HostingEnvironment.WebRootPath, @"Collect");

        // Uses the latest stored tourneys and compares with current tourneys
        var beforeThisDate = DateTime.MaxValue;

        if (!string.IsNullOrEmpty(id))
        {
            DateTime.TryParseExact(id, new[] {"dd'.'MM'.'yyyy", "dd'.'MM'.'yy", "yyyy'-'MM'-'dd"},
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out beforeThisDate);
        }
            
        var listModel = CollectionModelFactory.CreateListModel(beforeThisDate);
        return View(ViewName.Collect.Show, listModel);
    }
}
