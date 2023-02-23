using System;
using System.ComponentModel.DataAnnotations;

namespace TournamentCalendar.ValidationAttributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class TimeSpanDayTimeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return IsValid(value)
            ? null
            : new ValidationResult($"Wert für '{validationContext.DisplayName}' ist keine Uhrzeit");
    }

    public override bool IsValid(object? value)
    {
        if (value is not TimeSpan timeSpan)
            return false;

        return timeSpan >= new TimeSpan(0, 0, 0, 0) && timeSpan <= new TimeSpan(0, 23, 59, 59);
    }
}