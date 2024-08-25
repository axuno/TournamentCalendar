using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendar.Views;
using TournamentCalendar.Library;
using TournamentCalendar.Data;
using TournamentCalendar.Services;

namespace TournamentCalendar.Controllers;

[Route("volley-news")]
public class InfoService : ControllerBase
{
    private readonly string _domainName;
    private readonly IMailMergeService _mailMergeService;
    private readonly ILogger<InfoService> _logger;
    private readonly IAppDb _appDb;
    private readonly UserLocationService _locationService;

    public InfoService(IAppDb appDb, IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILogger<InfoService> logger, UserLocationService locationService, IMailMergeService mailMergeService) : base(hostingEnvironment, configuration)
    {
        _appDb = appDb;
        _locationService = locationService;
        _domainName = configuration["DomainName"]!;
        _mailMergeService = mailMergeService;
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToActionPermanent(nameof(InfoService.Register), nameof(InfoService));
    }

    [HttpGet(nameof(Register))]
    public IActionResult Register()
    {
        ViewBag.TitleTagText = "Volley-News abonnieren";
        var model = new Models.InfoService.EditModel { EditMode = Models.InfoService.EditMode.New };
        model.SetAppDb(_appDb);
        return View(ViewName.InfoService.Edit, model);
    }

    [HttpGet(nameof(Entry))]
    public IActionResult Entry()
    {
        return RedirectToAction(nameof(Register));
    }

    [HttpGet("entry/{guid}")]
    public IActionResult Entry(string guid)
    {
        ViewBag.TitleTagText = "Volley-News abonnieren";
        if (!ModelState.IsValid || string.IsNullOrEmpty(guid))
        {
            return RedirectToAction(nameof(InfoService.Index), nameof(Controllers.InfoService));
        }

        var model = new Models.InfoService.EditModel { EditMode = Models.InfoService.EditMode.Change };
        model.SetAppDb(_appDb, guid);

        if (!model.IsNew && !_locationService.GetLocation().IsSet)
            _locationService.SetGeoLocation(new UserLocation(model.Latitude, model.Longitude));

        return model.IsNew  // id not found
            ? RedirectToAction(nameof(InfoService.Index), nameof(Controllers.InfoService))
            : View(ViewName.InfoService.Edit, model);
    }

    /// <summary>
    /// "save" is the name of the submit button
    /// </summary>
    [HttpPost(nameof(InfoService.Entry)), ValidateAntiForgeryToken]
    public async Task<IActionResult> Entry([FromForm] Models.InfoService.EditModel model, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volley-News abonnieren";

        model.SetAppDb(_appDb);
        _ = await TryUpdateModelAsync(model);
        
        model.EditMode = string.IsNullOrWhiteSpace(model.Guid) ? Models.InfoService.EditMode.New : Models.InfoService.EditMode.Change;

        if (!ModelState.IsValid && model.ExistingEntryWithSameEmail is { ConfirmedOn: null })
            // if the entry with this email address was not yet confirmed, just redirect there
        {
            return RedirectToAction(nameof(InfoService.Entry), nameof(Controllers.InfoService), new {guid = model.ExistingEntryWithSameEmail.Guid });
        }

        // todo: what to do, if the email was already confirmed? Re-send confirmation email without asking?
        if (!ModelState.IsValid)
        {
            model.Normalize(ModelState);
            return View(ViewName.InfoService.Edit, model);
        }

        ModelState.Clear();
        if (model.EditMode == Models.InfoService.EditMode.Change
            && (!model.TryFetchEntity()
                || !await TryUpdateModelAsync<Models.InfoService.EditModel>(model)))
        {
            return View(ViewName.InfoService.Edit, model);
        }

        var googleApi = new GoogleConfiguration();
        Configuration.Bind(nameof(GoogleConfiguration), googleApi);
        await model.TryGetLongitudeLatitude(googleApi);
        model.Normalize();
			
        var confirmationModel = new Models.Shared.ConfirmModel<InfoServiceEntity>();

        try
        {
            HttpContext.Session.Remove(Axuno.Web.CaptchaSvgGenerator.CaptchaSessionKeyName);

            if (!(confirmationModel = await model.Save(cancellationToken)).SaveSuccessful)
                return View(ViewName.InfoService.Confirm, confirmationModel);

            if (confirmationModel.Entity?.UnSubscribedOn == null)
            {
                if (model is { Latitude: not null, Longitude: not null })
                    _locationService.SetGeoLocation(model.Latitude.Value, model.Longitude.Value);

                confirmationModel = await new Mailer(_mailMergeService, _domainName).MailInfoServiceRegistrationForm(confirmationModel,
                    Url.Action(nameof(Approve), nameof(Controllers.InfoService), new {guid = model.Guid})!,
                    Url.Action(nameof(Entry), nameof(Controllers.InfoService), new { guid = model.Guid})!);
            }

            return View(ViewName.InfoService.Confirm, confirmationModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email: {Email}, First/Last Name: {FirstName} {LastName}", model.Email, model.FirstName, model.LastName);
            return View(ViewName.InfoService.Confirm, confirmationModel);
        }
    }

    /// <summary>
    /// This method is called when using the submit button with attribute formaction = volley-news/unsubscribe
    /// </summary>
    [HttpPost(nameof(Unsubscribe))]
    public async Task<IActionResult> Unsubscribe([FromForm] Models.InfoService.EditModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            // We unregister the subscription
        }

        ViewBag.TitleTagText = "Volley-News abbestellen";
        var unsubscribeModel = new Models.InfoService.UnsubscribeModel(_appDb, model.Guid);
        return View(ViewName.InfoService.Unsubscribe, await unsubscribeModel.Save(cancellationToken));
    }

    [HttpGet("approve/{guid}")]
    public async Task<IActionResult> Approve(string guid, CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid)
            return View(ViewName.InfoService.Approve, false);

        ViewBag.TitleTagText = "Volley-News bestätigen";
        var approveModel = new Models.Shared.ApproveModelTournamentCalendar<InfoServiceEntity>(_appDb,InfoServiceFields.Guid == guid, InfoServiceFields.ConfirmedOn, InfoServiceFields.UnSubscribedOn);
        return View(ViewName.InfoService.Approve, await approveModel.Save(cancellationToken));
    }
}
