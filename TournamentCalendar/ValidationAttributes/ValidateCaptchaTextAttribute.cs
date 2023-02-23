using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Axuno.Web;
using Microsoft.Extensions.DependencyInjection;

namespace TournamentCalendar.ValidationAttributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ValidateCaptchaTextAttribute : ValidationAttribute
{
    private const string _defaultErrorMessage = "Lösung der Rechenaufgabe war nicht richtig.";

    public ValidateCaptchaTextAttribute()
        : base(_defaultErrorMessage)
    { }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var ctx = validationContext.GetService<IHttpContextAccessor>();

        if (ctx?.HttpContext?.User.Identity != null && ctx.HttpContext.User.Identity.IsAuthenticated)
        {
            return ValidationResult.Success!;
        }

        if (value is not string captchaText || ctx?.HttpContext?.Session.GetString(CaptchaSvgGenerator.CaptchaSessionKeyName) != captchaText)
        {
            return new ValidationResult(string.IsNullOrEmpty(ErrorMessage) ? _defaultErrorMessage : ErrorMessage);
        }

        return ValidationResult.Success!;
    }

    public override bool RequiresValidationContext => true;
}