using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TournamentCalendar.Resources;

namespace TournamentCalendar.Models.Contact;

[ModelMetadataType(typeof(ContactMetadata))]
public class ContactModel : ContactBaseModel
{
    public ContactModel()
    {
        Subject = Message = string.Empty;
    }

    public string? Subject { get; set; }
    public string? Message { get; set; }
    public bool CarbonCopyToSender { get; set; }


    public new void Normalize(ModelStateDictionary modelState)
    {
        /*
         * ASP.NET MVC assumes that if you’re rendering a View in response to an HTTP POST, 
         * and you’re using the Html Helpers, then you are most likely to be redisplaying a 
         * form that has failed validation. Therefore, the Html Helpers actually check in ModelState 
         * for the value to display in a field BEFORE they look in the Model. This enables them to 
         * redisplay erroneous data that was entered by the user, and a matching error message if needed.
         * BUT this way normalized values could not be shown to the user.
        */

        base.Normalize();
    }


    public new void Normalize()
    {
        base.Normalize();
        Subject = Subject?.Trim();
        Message = Message?.Trim();
    }


    [Bind("Gender, Title, FirstName, LastName, Email, Street, PostalCode, City, Fone, Subject, Message, Captcha, CarbonCopyToSender")]
    public class ContactMetadata : ContactBaseMetadata
    {

        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="Betreff")]
        public string? Subject { get; set; }

        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="Text")]
        public string? Message { get; set; }


        [Display(Name = "Kopie der Nachricht an mich verschicken")]
        public bool CarbonCopyToSender { get; set; }
    }
}