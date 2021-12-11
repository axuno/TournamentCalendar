using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TournamentCalendar.Models.Error;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers
{
    [Route("error")]
    public class Error : ControllerBase
    {
        private readonly ILogger _logger;

        public Error(ILogger<Error> logger, IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
        {
            _logger = logger;
        }

        [Route("{id}")]
        public IActionResult Index(string id)
        {
            ViewBag.TitleTagText = "Volleyball-Turnierkalender - Fehler";

            var viewModel = new ErrorModel();

            // The StatusCodePagesMiddleware stores a request-feature with
            // the original path on the HttpContext, that can be accessed from the Features property.
            // Note: IExceptionHandlerFeature does not contain the path
            var exceptionFeature = HttpContext.Features
                .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            if (exceptionFeature?.Error != null)
            {
                viewModel.OrigPath = exceptionFeature?.Path;
                viewModel.Exception = exceptionFeature?.Error;
                _logger.LogCritical(viewModel.Exception, "Path: {0}", viewModel.OrigPath);
            }
            else
            {
                viewModel.OrigPath = HttpContext.Features
                    .Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>()?.OriginalPath;
                _logger.LogInformation("Path: {0}, StatusCode: {1}", viewModel.OrigPath, id);
            }

            var statusCodes = XDocument.Load(System.IO.Path.Combine(HostingEnvironment.ContentRootPath,
                Program.ConfigurationFolder, "StatusCodes.config"));
            var status = (from item in statusCodes.Root?.Elements("statuscode")
                where !string.IsNullOrEmpty(id) && item.Element("code")?.Value == id
                select (
                    viewModel.Status.Code = (string) item.Element("code"),
                    viewModel.Status.Text = (string) item.Element("text"),
                    viewModel.Status.Description = (string) item.Element("description"),
                    viewModel.Status.GermanText = (string) item.Element("germantext"),
                    viewModel.Status.GermanDescription = (string) item.Element("germandescription")
                    )).FirstOrDefault();

            if (string.IsNullOrEmpty(viewModel.Status.Code))
            {
                viewModel.Status.Code = "500";
                viewModel.Status.Text = "Server Error";
                viewModel.Status.Description = "Unkown Server Error occurred.";
                viewModel.Status.GermanText = "Serverfehler";
                viewModel.Status.GermanDescription = "Ein unbekannter Serverfehler ist aufgetreten.";
            }

            return View(ViewName.Error.Index, viewModel);
        }
    }
}