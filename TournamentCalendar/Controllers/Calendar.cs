﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TournamentCalendar.Library;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendar.Models.Calendar;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers
{
    [Route("")]
    [Route("Kalender")]
    public class Calendar : ControllerBase
    {
        private readonly IMailMergeService _mailMergeService;
        private readonly string _domainName;
        private readonly ILogger<Calendar> _logger;

        public Calendar(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILogger<Calendar> logger, IMailMergeService mailMergeService) : base(hostingEnvironment, configuration)
        {
            _mailMergeService = mailMergeService;
            _domainName = configuration["DomainName"];
            _logger = logger;
        }

        [Route("")]
        public async Task<IActionResult> Kalender()
        {
            ViewBag.TitleTagText = "Volleyball-Turnierkalender";
		    var model = new BrowseModel();
		    await model.Load();
            return View(ViewName.Calendar.Overview, model);
        }

        [Route("Id/{id:long}")]
		public async Task<IActionResult> Id(long id)
		{
		    ViewBag.TitleTagText = "Volleyball-Turnierkalender";
		    var model = new BrowseModel();
		    try
		    {
		        await model.Load(id);
            }
		    catch (ArgumentOutOfRangeException)
		    {
                // no tournaments found for "id"
		        return new StatusCodeResult(404);
		    }
		    
            return View(ViewName.Calendar.Show, model);
		}

        [HttpGet]
        [Route("eintrag/{id?}")]
        public async Task<IActionResult> Entry(string id)
        {
            ViewBag.TitleTagText = "Volleyballturnier in den Kalender eintragen";

            var model = await new EditModel().Initialize();
            if (string.IsNullOrEmpty(id))
            {
                model.EditMode = EditMode.New;
                return View(ViewName.Calendar.Edit, model);
            }

            model.EditMode = EditMode.Change;
            model.LoadTournament(id);

            return model.IsNew  // id not found
                ? (ActionResult)RedirectToAction(nameof(Calendar.Entry), nameof(Controllers.Calendar), new { id = string.Empty })
                : View(ViewName.Calendar.Edit, model);
        }

        [HttpPost]
		[Route("eintrag/{id?}")]
        public async Task<IActionResult> Entry(EditModel model)
		{
			ViewBag.TitleTagText = "Volleyballturnier in den Kalender eintragen";
		    await model.Initialize();

			model.EditMode = string.IsNullOrWhiteSpace(model.Guid) ? EditMode.New : EditMode.Change;

			if (!ModelState.IsValid)
			{
                return View(ViewName.Calendar.Edit, model);
			}
            
			if (model.EditMode == EditMode.Change)
			{
				if (model.TryRefetchEntity())
				{
					if (! await TryUpdateModelAsync<EditModel>(model))
					{
						return View(ViewName.Calendar.Edit, model);
					}
				}
			}

			model.Normalize(ModelState);
            var possibleDupe = await model.GetPossibleDuplicate();
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
            if (model.IsNew && User.Identity != null && User.Identity.IsAuthenticated)
            {
                model.CreatedByUser = User.Identity.Name;
            }
			
			var confirmationModel = new Models.Shared.ConfirmModel<CalendarEntity>();

			try
			{
			    HttpContext.Session.Remove(Axuno.Web.CaptchaSvgGenerator.CaptchaSessionKeyName);

				if ((confirmationModel = await model.Save()).SaveSuccessful)
				{
					if (!confirmationModel.Entity!.ApprovedOn.HasValue || DateTime.Now - confirmationModel.Entity.ApprovedOn.Value < new TimeSpan(0,1,0))
					{
						confirmationModel = await
						new Mailer(_mailMergeService, _domainName).MailTournamentCalendarForm(confirmationModel,
									Url.Action(nameof(Approve), nameof(Controllers.Calendar), new {id = model.Guid})!,
									Url.Action(nameof(Entry), nameof(Controllers.Calendar), new { id = model.Guid })!,
									Url.Action(nameof(Id), new {id = model.Id})!);
					}
				}

				return View(ViewName.Calendar.Confirm, confirmationModel);
			}
			catch (Exception ex)
			{
			    _logger.LogError(ex, "Email: {PostedByEmail}, PostedBy: {PostedByName},  TournamentName: {TournamentName}", model.PostedByEmail, model.PostedByName, model.TournamentName);
                return View(ViewName.Calendar.Confirm, confirmationModel);
			}
		}

        [Route("bestaetigen/{id?}")]
        public async Task<IActionResult> Approve(string id = "")
		{
		    ViewBag.TitleTagText = "Volleyball-Turniereintrag bestätigen";
            var approveModel = new Models.Shared.ApproveModelTournamentCalendar<CalendarEntity>(CalendarFields.Guid == id, CalendarFields.ApprovedOn, CalendarFields.DeletedOn);
			
			return View(ViewName.Calendar.Approve, await approveModel.Save());
		}

        [Route("integrieren")]
        public IActionResult Integrate()
        {
            ViewBag.TitleTagText = "Turnierkalender integrieren";

            return View(ViewName.Calendar.Integrate);
        }
    }
}
