using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using TournamentCalendar.Resources;

namespace TournamentCalendar.Models.Contact;

public enum ContactType
{
    Message, ArticlesOfAssociation
}

[ModelMetadataType(typeof(ContactBaseMetadata))]
public class ContactBaseModel : IValidatableObject
{
    public ContactBaseModel()
    {
    }

    public string? Gender { get; set; }
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Fone { get; set; }

    public string? Captcha { get; set; }

    [BindNever]
    public bool EmailSuccessFul { get; set; }

    [BindNever]
    public Exception? Exception { get; set; }

    public IEnumerable<SelectListItem> GetGenderList()
    {
        return new List<SelectListItem>(3)
        {
            new() {Value = "", Text = "Bitte wählen"},
            // a value of string.Empty will cause a required validation error
            new() {Value = "f", Text = "Frau"},
            new() {Value = "m", Text = "Herr"},
            new() {Value = "u", Text = "keine"}
        };
    }

    public void Normalize(ModelStateDictionary _)
    {
        /*
         * ASP.NET MVC assumes that if you’re rendering a View in response to an HTTP POST, 
         * and you’re using the Html Helpers, then you are most likely to be redisplaying a 
         * form that has failed validation. Therefore, the Html Helpers actually check in ModelState 
         * for the value to display in a field BEFORE they look in the Model. This enables them to 
         * redisplay erroneous data that was entered by the user, and a matching error message if needed.
         * BUT this way normalized values could not be shown to the user.
        */

        Normalize();
    }


    public void Normalize()
    {
        if (Gender != null && Gender != "f" && Gender != "m") Gender = "u";
        Title = Title?.Trim() ?? string.Empty;
        FirstName = FirstName?.Trim() ?? string.Empty;
        LastName = LastName?.Trim() ?? string.Empty;
        Email = Email?.Trim() ?? string.Empty;
        Street = Street?.Trim() ?? string.Empty;
        PostalCode = PostalCode?.Trim() ?? string.Empty;
        City = City?.Trim() ?? string.Empty;
        Fone = Fone?.Trim() ?? string.Empty;
    }


    [Bind("Gender, Title, FirstName, LastName, Email, Street, PostalCode, City, Fone, Captcha")]
    public class ContactBaseMetadata
    {
        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="Anrede")]
        public string? Gender { get; set; }

        [Display(Name="Titel")]
        public string? Title { get; set; }

        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="Vorname")]
        public string? FirstName { get; set; }

        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="Familienname")]
        public string? LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression(
            @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$"
            , ErrorMessage = "E-Mail hat ungültiges Format")]
        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="E-Mail")]
        public string? Email { get; set; }

        [Display(Name="Straße")]
        public string? Street { get; set; }

        [Display(Name="Postleitzahl")]
        public string? PostalCode { get; set; }

        [Display(Name="Ort")]
        public string? City { get; set; }

        [Display(Name="Telefon")]
        public string? Fone { get; set; }

        [Display(Name = "Ergebnis der Rechenaufgabe im Bild")]
        [ValidationAttributes.ValidateCaptchaText]
        public string? Captcha { get; set; }
    }

    #region IValidatableObject Members

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Will be called only after individual fields are valid
        var errors = new List<ValidationResult>();

        return errors;
    }

    #endregion
}
