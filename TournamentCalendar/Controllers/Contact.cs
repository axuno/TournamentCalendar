using System;
using System.Threading.Tasks;
using TournamentCalendar.Models.Contact;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers
{
    [Route("kontakt")]
    public class Contact : ControllerBase
	{
	    private readonly string _domainName;
	    private readonly IMailMergeService _mailMergeService;

	    private readonly ILogger<Contact> _logger;

        public Contact(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILogger<Contact> logger, IMailMergeService mailMergeService) : base(hostingEnvironment, configuration)
	    {
	        _domainName = configuration["DomainName"];
	        _mailMergeService = mailMergeService;
	        _logger = logger;
        }

	    [HttpGet]
	    [Route("")]
        public IActionResult Index()
		{
            return RedirectToAction(nameof(Message));
		}

        [HttpGet]
        [Route("nachricht")]
        public async Task<IActionResult> Message()
		{
			ViewBag.TitleTagText = "Volleyball-Turnier.de kontaktieren";
		    return await Task.Run(() =>
		    {
		        var model = new ContactModel();
		        return View(ViewName.Contact.Message, model);
		    });
		}

	    [HttpPost("nachricht")]
        public async Task<IActionResult> Message([FromForm] ContactModel model)
		{
			ViewBag.TitleTagText = "Volleyball-Turnier.de kontaktieren";

		    if (!ModelState.IsValid)
		    {
		        model.Normalize(ModelState);
		        return View(ViewName.Contact.Message, model);
		    }

		    model.Normalize();
		    
		    try
		    {
		        HttpContext.Session.Remove(Axuno.Web.CaptchaSvgGenerator.CaptchaSessionKeyName);
                model = await new Mailer(_mailMergeService, _domainName).ContactForm(model, Url.Action(nameof(this.Message), nameof(Controllers.Contact)));
		        return View(ViewName.Contact.Confirm, model);
		    }
		    catch (Exception ex)
		    {
		        _logger.LogError(ex, $"Email: {model.Email}, Name: {model.FirstName} {model.LastName}, Subject: {model.Subject}, Message: {model.Message}");
		        return View(ViewName.Contact.Confirm, model);
		    }
		}
    }
}
