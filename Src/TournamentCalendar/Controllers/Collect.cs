using TournamentCalendar.Collecting;
using TournamentCalendar.Data;
using TournamentCalendar.Models.Collect;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Authorize(Roles = Library.Authentication.Constants.RoleName.Editor)]
[Route(nameof(Collect))]
public class Collect : ControllerBase
{
    private readonly IAppDb _appDb;

    public Collect(IAppDb appDb, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(
        hostingEnvironment, configuration)
    {
        _appDb = appDb;
    }

    [HttpGet("show/{id?}")]
    public async Task<IActionResult> Show(string? id)
    {
        if (!ModelState.IsValid)
            id = null;

        ViewBag.TitleTagText = "Andere Volleyball-Turnierkalender";

        Storage.StorageFolder = Path.Combine(HostingEnvironment.WebRootPath, Storage.StorageFolderName);

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
