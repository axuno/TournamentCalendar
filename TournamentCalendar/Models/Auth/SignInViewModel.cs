using System.ComponentModel.DataAnnotations;
using TournamentCalendar.Resources;

namespace TournamentCalendar.Models.AccountViewModels;

public class SignInViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired), ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Display(Name = "E-Mail oder Benutzername")]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string? EmailOrUsername { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired), ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Password)]
    [Display(Name = "Passwort")]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string? Password { get; set; }

    [Display(Name = "Angemeldet bleiben?")]
    public bool RememberMe { get; set; }
}