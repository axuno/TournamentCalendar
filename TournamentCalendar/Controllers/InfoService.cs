﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendar.Models.InfoService;
using TournamentCalendar.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TournamentCalendar.Library;
using System.Threading;

namespace TournamentCalendar.Controllers;

[Route("volley-news")]
public class InfoService : ControllerBase
{
    private readonly string _domainName;
    private readonly IMailMergeService _mailMergeService;
    private readonly ILogger<InfoService> _logger;

    public InfoService(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILogger<InfoService> logger, IMailMergeService mailMergeService) : base(hostingEnvironment, configuration)
    {
        _domainName = configuration["DomainName"];
        _mailMergeService = mailMergeService;
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.TitleTagText = "Volley-News abonnieren";
        return View(ViewName.InfoService.Edit, new EditModel { EditMode = EditMode.New });
    }

    [HttpGet("[action]/{id?}")]
    public IActionResult Eintrag(string id)
    {
        ViewBag.TitleTagText = "Volley-News abonnieren";
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(InfoService.Index), nameof(Controllers.InfoService));
        }

        var model = new EditModel(id) { EditMode = EditMode.Change };
        return model.IsNew  // id not found
            ? (IActionResult) RedirectToAction(nameof(InfoService.Index), nameof(Controllers.InfoService))
            : View(ViewName.InfoService.Edit, model);
    }

    /// <summary>
    /// "save" is the name of the submit button
    /// </summary>
    [HttpPost("[action]/{id?}")]
    public async Task<IActionResult> Eintrag([FromForm] EditModel model, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volley-News abonnieren";
        model.EditMode = string.IsNullOrWhiteSpace(model.Guid) ? EditMode.New : EditMode.Change;

        if (!ModelState.IsValid && model.ExistingEntryWithSameEmail != null)
        {
            // if the entry with this email address was not yet confirmed, just redirect there
            if (!model.ExistingEntryWithSameEmail.ConfirmedOn.HasValue)
            {
                return RedirectToAction(nameof(InfoService.Eintrag), nameof(Controllers.InfoService), new {id = model.ExistingEntryWithSameEmail.Guid });
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
                if (!TryUpdateModelAsync<EditModel>(model).Result)
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
                        Url.Action(nameof(Bestaetigen), nameof(Controllers.InfoService), new {id = model.Guid})!,
                        Url.Action(nameof(Eintrag), nameof(Controllers.InfoService), new {id = model.Guid})!);
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
    /// This method is call when using the submit button with attribute formaction = volley-news/unsubscribe
    /// </summary>
    [HttpPost("[action]/{id?}")]
    public async Task<IActionResult> Unsubscribe([FromForm] EditModel model, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volley-News abbestellen";
        var unsubscribeModel = new Models.InfoService.UnsubscribeModel(model.Guid);
        return View(ViewName.InfoService.Unsubscribe, await unsubscribeModel.Save(cancellationToken));
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> Bestaetigen(string id, CancellationToken cancellationToken)
    {
        ViewBag.TitleTagText = "Volley-News bestätigen";
        var approveModel = new Models.Shared.ApproveModelTournamentCalendar<InfoServiceEntity>(InfoServiceFields.Guid == id, InfoServiceFields.ConfirmedOn, InfoServiceFields.UnSubscribedOn);
        return View(ViewName.InfoService.Approve, await approveModel.Save(cancellationToken));
    }
}
