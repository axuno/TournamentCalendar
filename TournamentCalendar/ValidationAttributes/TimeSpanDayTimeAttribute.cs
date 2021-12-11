using System;
using System.ComponentModel.DataAnnotations;

namespace TournamentCalendar.ValidationAttributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class TimeSpanDayTimeAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			return IsValid(value)
					   ? null
					   : new ValidationResult(string.Format("Wert für '{0}' ist keine Uhrzeit", validationContext.DisplayName));
		}

		public override bool IsValid(object value)
		{
			if (!(value is TimeSpan))
				return false;

			var ts = (TimeSpan)value;
			return ts >= new TimeSpan(0, 0, 0, 0) && ts <= new TimeSpan(0, 23, 59, 59);
		}
	}
}