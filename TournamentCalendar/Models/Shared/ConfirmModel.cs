using System;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace TournamentCalendar.Models.Shared
{
	public class ConfirmModel<T> where T : EntityBase2
	{
		public bool SaveSuccessful { get; set; }
		
		/// <summary>
		/// Indicates, whether an email was sent succsssfully
		/// Value is null if email is not required
		/// </summary>
		public bool? EmailSuccessful { get; set; }
		
		public Exception Exception { get; set; }
		public T Entity { get; set; }
		public string ReturnUrl { get; set; }
	}
}