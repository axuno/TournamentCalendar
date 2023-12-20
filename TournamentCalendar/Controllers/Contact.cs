using TournamentCalendar.Models.Contact;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

[Route("contact")]
public class Contact : ControllerBase
{
    private readonly string _domainName;
    private readonly IMailMergeService _mailMergeService;

    private readonly ILogger<Contact> _logger;

    public Contact(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILogger<Contact> logger, IMailMergeService mailMergeService) : base(hostingEnvironment, configuration)
    {
        _domainName = configuration["DomainName"]!;
        _mailMergeService = mailMergeService;
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToActionPermanent(nameof(Message));
    }

    [HttpGet(nameof(Message))]
    public IActionResult Message()
    {
        ViewBag.TitleTagText = "Volleyball-Turnier.de kontaktieren";
        var model = new ContactModel();
        return View(ViewName.Contact.Message, model);
    }

    [HttpPost(nameof(Message)), ValidateAntiForgeryToken]
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
            model = await new Mailer(_mailMergeService, _domainName).ContactForm(model, Url.Action(nameof(Message), nameof(Controllers.Contact))!);
            return View(ViewName.Contact.Confirm, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email: {Email}, Name: {FirstName} {LastName}, Subject: {Subject}, Message: {Message}", model.Email, model.FirstName, model.LastName, model.Subject, model.Message);
            return View(ViewName.Contact.Confirm, model);
        }
    }
}
