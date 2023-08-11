using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendar.Models.InfoService;
using TournamentCalendar.Views;
using TournamentCalendar.Library;
using TournamentCalendar.Data;

namespace TournamentCalendar.Controllers;

[Route("volley-news")]
public class InfoService : ControllerBase
{
    private readonly string _domainName;
    private readonly IMailMergeService _mailMergeService;
    private readonly ILogger<InfoService> _logger;
    private readonly IAppDb _appDb;

    public InfoService(IAppDb appDb, IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILogger<InfoService> logger, IMailMergeService mailMergeService) : base(hostingEnvironment, configuration)
    {
        _appDb = appDb;
        _domainName = configuration["DomainName"];
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
        return View(ViewName.InfoService.Edit, new EditModel(_appDb) { EditMode = EditMode.New });
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
        if (string.IsNullOrEmpty(guid))
        {
            return RedirectToAction(nameof(InfoService.Index), nameof(Controllers.InfoService));
        }

        var model = new EditModel(_appDb, guid) { EditMode = EditMode.Change };
        return model.IsNew  // id not found
            ? RedirectToAction(nameof(InfoService.Index), nameof(Controllers.InfoService))
            : View(ViewName.InfoService.Edit, model);
    }

    /// <summary>
    /// "save" is the name of the submit button
    /// </summary>
    [HttpPost(nameof(InfoService.Entry)), ValidateAntiForgeryToken]
    public async Task<IActionResult> Entry([FromForm] EditModel model, CancellationToken cancellationToken)
    {
        model = new EditModel(_appDb);
        _ = await TryUpdateModelAsync<EditModel>(model);

        ViewBag.TitleTagText = "Volley-News abonnieren";
        model.EditMode = string.IsNullOrWhiteSpace(model.Guid) ? EditMode.New : EditMode.Change;

        if (!ModelState.IsValid && model.ExistingEntryWithSameEmail != null)
        {
            // if the entry with this email address was not yet confirmed, just redirect there
            if (!model.ExistingEntryWithSameEmail.ConfirmedOn.HasValue)
            {
                return RedirectToAction(nameof(InfoService.Entry), nameof(Controllers.InfoService), new {guid = model.ExistingEntryWithSameEmail.Guid });
            }

            // todo: what to do, if the email was already confirmed? Re-send confirmation email without asking?
        }

        if (!ModelState.IsValid)
        {
            model.Normalize(ModelState);
            return View(ViewName.InfoService.Edit, model);
        }

        ModelState.Clear();
        if (model.EditMode == EditMode.Change)
            if (model.TryRefetchEntity())
                if (!await TryUpdateModelAsync<EditModel>(model))
                    return View(ViewName.InfoService.Edit, model);

        var googleApi = new GoogleConfiguration();
        Configuration.Bind(nameof(GoogleConfiguration), googleApi);
        await model.TryGetLongitudeLatitude(googleApi);
        model.Normalize();
			
        var confirmationModel = new Models.Shared.ConfirmModel<InfoServiceEntity>();

        try
        {
            HttpContext.Session.Remove(Axuno.Web.CaptchaSvgGenerator.CaptchaSessionKeyName);

            if ((confirmationModel = await model.Save(cancellationToken)).SaveSuccessful)
            {
                if (confirmationModel.Entity?.UnSubscribedOn == null)
                {
                    confirmationModel = await new Mailer(_mailMergeService, _domainName).MailInfoServiceRegistrationForm(confirmationModel,
                        Url.Action(nameof(Approve), nameof(Controllers.InfoService), new {guid = model.Guid})!,
                        Url.Action(nameof(Entry), nameof(Controllers.InfoService), new { guid = model.Guid})!);
                }
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
    public async Task<IActionResult> Unsubscribe([FromForm] EditModel model, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volley-News abbestellen";
        var unsubscribeModel = new Models.InfoService.UnsubscribeModel(_appDb, model.Guid);
        return View(ViewName.InfoService.Unsubscribe, await unsubscribeModel.Save(cancellationToken));
    }

    [HttpGet("approve/{guid}")]
    public async Task<IActionResult> Approve(string guid, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volley-News bestätigen";
        var approveModel = new Models.Shared.ApproveModelTournamentCalendar<InfoServiceEntity>(_appDb,InfoServiceFields.Guid == guid, InfoServiceFields.ConfirmedOn, InfoServiceFields.UnSubscribedOn);
        return View(ViewName.InfoService.Approve, await approveModel.Save(cancellationToken));
    }
}
