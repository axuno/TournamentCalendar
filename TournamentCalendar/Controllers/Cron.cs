using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace TournamentCalendar.Controllers
{
	/// <summary>
	/// CronController
	/// </summary>
	public class Cron : ControllerBase
    {
        public ContentResult Index()
		{
			return Content("<pre>This page is accessible only via cron job.</pre>");
		}

		#region *** Heartbeat ***

		private const string _internalPing = "internal-ping";

		/// <summary>
		/// Used by external cron job (every 1-5 minutes)
		/// in order to prevent application idle timeout shutdowns 
		/// </summary>
		/// <returns></returns>
		public ContentResult Heartbeat(string id)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
			return Content($"Completed - {DateTime.Now:G}.", "text/plain", System.Text.Encoding.UTF8);
		}

		/// <summary>
		/// Heartbeat url is used by application_end event in global.asax
		/// in order to initiate application restart by IIS.
		/// </summary>
		/// <returns>Returns the ping url to initiate application restart by IIS</returns>
		public static string GetHeartbeatUrl()
		{
            return "https://volleyball-turnier.de/cron/heartbeat/" + _internalPing;
		}

		#endregion
	}
}
