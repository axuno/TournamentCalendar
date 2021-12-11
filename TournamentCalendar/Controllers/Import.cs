using System;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TournamentCalendar.Models.TournamentImport;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers
{
    [Authorize(Roles = Library.Authentication.Constants.RoleName.Editor)]
    [Route("import")]
    public class Import : ControllerBase
	{
		public Import(IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(hostingEnvironment, configuration)
		{}

        [Route("anzeigen/{id?}")]
        public ActionResult Show(string id)
		{
		    ViewBag.TitleTagText = "Andere Volleyball-Turnierkalender";

            var model = new ImportModel(Path.Combine(HostingEnvironment.WebRootPath, @"App_Data\Import"));
		    var keyDate = DateTime.MaxValue;

		    if (!string.IsNullOrEmpty(id))
		    {
		        DateTime.TryParseExact(id, new[] {"dd'.'MM'.'yyyy", "dd'.'MM'.'yy", "yyyy'-'MM'-'dd"},
		            CultureInfo.InvariantCulture,
		            DateTimeStyles.None,
		            out keyDate);
		    }
            
		    model.GetTournamentsAfterKeyDate(out var listModel, keyDate, new[] { Provider.Volleyballer, Provider.Vobatu });
            return View(ViewName.TournamentImport.Show, listModel);
		}
	}
}
