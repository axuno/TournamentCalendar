using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TournamentCalendar.Collectors;
using TournamentCalendar.Data;
using TournamentCalendar.Models.Collect;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Authorize(Roles = Library.Authentication.Constants.RoleName.Editor)]
[Route("import")]
public class Collect : ControllerBase
{
    private readonly IAppDb _appDb;

    public Collect(IAppDb appDb, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(
        hostingEnvironment, configuration)
    {
        _appDb = appDb;
    }

    [HttpGet("anzeigen/{id?}")]
    public async Task<IActionResult> Show(string id)
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
            
        var listModel = await CollectionModelFactory.CreateListModel(beforeThisDate, _appDb);
        return View(ViewName.Collect.Show, listModel);
    }
}
