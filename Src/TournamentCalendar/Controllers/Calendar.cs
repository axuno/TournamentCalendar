using TournamentCalendar.Library;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendar.Models.Calendar;
using TournamentCalendar.Views;
using TournamentCalendar.Data;
using TournamentCalendar.Services;

namespace TournamentCalendar.Controllers;

[Route(nameof(Calendar))]
public class Calendar : ControllerBase
{
    private readonly IMailMergeService _mailMergeService;
    private readonly string _domainName;
    private readonly UserLocation _userLocation;
    private readonly ILogger<Calendar> _logger;
    private readonly IAppDb _appDb;

    public Calendar(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, IAppDb appDb, ILogger<Calendar> logger, IMailMergeService mailMergeService, UserLocationService locationService) : base(hostingEnvironment, configuration)
    {
        _mailMergeService = mailMergeService;
        _domainName = configuration["DomainName"]!;
        _userLocation = locationService.GetLocation();
        _appDb = appDb;
        _logger = logger;
    }

    [HttpGet("/")]
    public IActionResult Index(CancellationToken cancellationToken)
    {
        return RedirectToActionPermanent(nameof(All), nameof(Calendar));
    }

    [HttpGet("")]
    public async Task<IActionResult> All(CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volleyball-Turnierkalender";
        var model = new BrowseModel(_appDb, _userLocation);
        await model.Load(cancellationToken);
        
        return View(ViewName.Calendar.Overview, model);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Id(long id, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volleyball-Turnierkalender";

        if (!ModelState.IsValid)
            return new StatusCodeResult(404);

        var model = new BrowseModel(_appDb, _userLocation);
        try
        {
            await model.Load(id, cancellationToken);
        }
        catch (ArgumentOutOfRangeException)
        {
            // no tournaments found for "id"
            return new StatusCodeResult(404);
        }
		    
        return View(ViewName.Calendar.Show, model);
    }

    [HttpGet("entry")]
    public async Task<IActionResult> NewEntry(CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volleyballturnier in den Kalender eintragen";
        var model = await new EditModel().Initialize(_appDb, cancellationToken);
        model.Guid = string.Empty;
        model.EditMode = EditMode.New;
        return View(ViewName.Calendar.Edit, model);
    }

    [HttpGet("entry/{guid}")]
    public async Task<IActionResult> Entry([FromRoute] string guid, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volleyballturnier in den Kalender eintragen";

        if (!ModelState.IsValid || (!string.IsNullOrEmpty(guid) && !Guid.TryParse(guid, out _)))
        {
            return NotFound();
        }

        var model = await new EditModel().Initialize(_appDb, cancellationToken);
        if (string.IsNullOrEmpty(guid))
        {
            model.EditMode = EditMode.New;
            model.Guid = string.Empty;
            return View(ViewName.Calendar.Edit, model);
        }

        model.EditMode = EditMode.Change;
        model.LoadTournament(guid, cancellationToken);

        return model.IsNew  // guid not found
            ? RedirectToAction(nameof(Calendar.Entry), nameof(Controllers.Calendar), new { guid = string.Empty })
            : View(ViewName.Calendar.Edit, model);
    }

    [HttpPost(nameof(Entry)), ValidateAntiForgeryToken]
    public async Task<IActionResult> Entry([FromForm] EditModel model, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volleyballturnier in den Kalender eintragen";
        await model.Initialize(_appDb, cancellationToken);

        model.EditMode = string.IsNullOrWhiteSpace(model.Guid) ? EditMode.New : EditMode.Change;

        if (!ModelState.IsValid)
        {
            return View(ViewName.Calendar.Edit, model);
        }
            
        if (model.EditMode == EditMode.Change)
        {
            if (model.TryRefetchEntity(cancellationToken))
            {
                if (! await TryUpdateModelAsync<EditModel>(model))
                {
                    return View(ViewName.Calendar.Edit, model);
                }
            }
            else
            {
                // Tournament for model.Guid does not exist
                return RedirectToAction(nameof(Calendar.Entry), nameof(Controllers.Calendar));
            }
        }

        model.Normalize(ModelState);
        var possibleDupe = await model.GetPossibleDuplicate(cancellationToken);
        if (possibleDupe != null)
        {
            ModelState.Clear();
            ModelState.AddModelError(string.Empty, $"Es ist bereits ein Turnier ({possibleDupe.TournamentSurface.Description}) am {possibleDupe.DateFrom.ToShortDateString()}, PLZ {possibleDupe.PostalCode}, für {possibleDupe.NumPlayersFemale} Damen / {possibleDupe.NumPlayersMale} Herren vorhanden.");
            return View(ViewName.Calendar.Edit, model);
        }

        var googleApi = new GoogleConfiguration();
        Configuration.Bind(nameof(GoogleConfiguration), googleApi);
        await model.TryGetLongitudeLatitude(googleApi);
        model.Normalize();
        if (model.IsNew && User.Identity is { IsAuthenticated: true })
        {
            model.CreatedByUser = User.Identity.Name;
        }
			
        var confirmationModel = new Models.Shared.ConfirmModel<CalendarEntity>();

        try
        {
            HttpContext.Session.Remove(Axuno.Web.CaptchaSvgGenerator.CaptchaSessionKeyName);

            if ((confirmationModel = await model.Save(cancellationToken)).SaveSuccessful)
            {
                if (!confirmationModel.Entity!.ApprovedOn.HasValue ||
                    DateTime.Now - confirmationModel.Entity.ApprovedOn.Value < new TimeSpan(0, 1, 0))
                {
                    confirmationModel = await
                        new Mailer(_mailMergeService, _domainName).MailTournamentCalendarForm(confirmationModel,
                            Url.Action(nameof(Approve), nameof(Controllers.Calendar), new { guid = model.Guid })!,
                            Url.Action(nameof(Entry), nameof(Controllers.Calendar), new { guid = model.Guid })!,
                            Url.Action(nameof(Id), new { id = model.Id })!);
                }
            }

            return View(ViewName.Calendar.Confirm, confirmationModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email: {PostedByEmail}, PostedBy: {PostedByName},  TournamentName: {TournamentName}",
                model.PostedByEmail, model.PostedByName, model.TournamentName);
            return View(ViewName.Calendar.Confirm, confirmationModel);
        }
    }

    [HttpGet("approve/{guid?}")]
    public async Task<IActionResult> Approve(string? guid, CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid)
            return View(ViewName.Calendar.Approve, false);

        guid ??= string.Empty;
        ViewBag.TitleTagText = "Volleyball-Turniereintrag bestätigen";
        var approveModel = new Models.Shared.ApproveModelTournamentCalendar<CalendarEntity>(_appDb,CalendarFields.Guid == guid, CalendarFields.ApprovedOn, CalendarFields.DeletedOn);
			
        return View(ViewName.Calendar.Approve, await approveModel.Save(cancellationToken));
    }

    [HttpGet(nameof(Integrate))]
    public IActionResult Integrate()
    {
        ViewBag.TitleTagText = "Turnierkalender integrieren";

        return View(ViewName.Calendar.Integrate);
    }
}
