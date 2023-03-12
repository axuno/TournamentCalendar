using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using TournamentCalendar.Resources;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendar.Library;
using System.Threading;

namespace TournamentCalendar.Models.InfoService;

public enum EditMode
{
    New, Change
}

[ModelMetadataType(typeof (InfoServiceMetadata))]
public class EditModel : InfoServiceEntity, IValidatableObject
{
    private bool _isAddressEntered = true;
    private readonly IAppDb? _appDb;

    public EditModel()
    {
        base.Guid = string.Empty;
        // someone who saves changed data is considered as subscriber
        base.UnSubscribedOn = null;
        base.Gender = "";
        base.CountryId = "";
        base.IsNew = true;
        IsAddressEntered = true;
    }

    public EditModel(IAppDb appDb) : this()
    {
        _appDb = appDb;
    }

    public EditModel(IAppDb appDb, string guid)
    {
        _appDb = appDb;
        LoadData(guid);
        IsAddressEntered = true;
    }

    private bool LoadData(string guid)
    {
        return _appDb!.InfoServiceRepository.GetRegistrationByGuid(this, guid);
    }

    public bool TryRefetchEntity()
    {
        return string.IsNullOrWhiteSpace(Guid) || LoadData(Guid);
    }

    public async Task<bool> TryGetLongitudeLatitude(GoogleConfiguration googleConfig)
    {
        if (Fields[InfoServiceFields.Street.FieldIndex].IsChanged || Fields[InfoServiceFields.ZipCode.FieldIndex].IsChanged ||
            Fields[InfoServiceFields.City.FieldIndex].IsChanged)
        {
            try
            {
                // try to get longitude and latitude by Google Maps API
                var completeAddress = string.Join(", ", ZipCode, City, Street);

                var location = await Axuno.Tools.GeoSpatial.GoogleGeo.GetLocation(CountryId, completeAddress, googleConfig.ServiceApiKey, TimeSpan.FromSeconds(15));
                if (location.GeoLocation.Latitude != null && location.GeoLocation.Latitude.Degrees > 1 && location.GeoLocation.Longitude?.Degrees > 1)
                {
                    Longitude = location.GeoLocation.Longitude.TotalDegrees;
                    Latitude = location.GeoLocation.Latitude.TotalDegrees;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        return true;
    }

    public int GetDistanceToAugsburg()
    {
        if (Longitude.HasValue && Latitude.HasValue)
        {
            // Distance to Augsburg/Königsplatz
            var augsburg =
                new Axuno.Tools.GeoSpatial.Location(new Axuno.Tools.GeoSpatial.Latitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(48.3666)),
                    new Axuno.Tools.GeoSpatial.Longitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(10.894103)));

            var userLoc =
                new Axuno.Tools.GeoSpatial.Location(
                    new Axuno.Tools.GeoSpatial.Latitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(Latitude.Value)),
                    new Axuno.Tools.GeoSpatial.Longitude(Axuno.Tools.GeoSpatial.Angle.FromDegrees(Longitude.Value)));

            return (int) (userLoc.Distance(augsburg) + 500)/1000;
        }
        return 0;
    }

    public IEnumerable<SelectListItem> GetGenderList()
    {
        return new List<SelectListItem>(3)
        {
            new() {Value = "", Text = "Bitte wählen"},
            // a value of string.Empty will cause a required validation error
            new() {Value = "f", Text = "Frau"},
            new() {Value = "m", Text = "Herr"}
        };
    }


    public EditMode EditMode
    { get; set; }


    public bool IsAddressEntered
    {
        get { return _isAddressEntered; }
        set
        {
            _isAddressEntered = value;
            if (!_isAddressEntered)
            {
                base.CountryId = base.ZipCode = base.City = base.Street = string.Empty;
                base.MaxDistance = 0;
            }
        }
    }

    public InfoServiceEntity? ExistingEntryWithSameEmail { get; private set; }

    public string Captcha { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> GetCountriesList()
    {
        var countryIds = new[] { "DE", "AT", "CH", "LI", "IT", "NL", "BE", "LU", "FR", "PL", "DK", "CZ", "SK" };

        var countries = new EntityCollection<CountryEntity>();
        _appDb!.CountriesRepository.GetCountriesList(countries, countryIds);

        // add to countries list in the sequence of countryIds array
        return countryIds.Select(id => countries.First(c => c.Id == id)).Select(
            item => new SelectListItem {Value = item.Id, Text = item.Name}).ToList();
    }

    public void Normalize(ModelStateDictionary modelState)
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
        for (var i = 0; i < Fields.Count; i++)
        {
            var fieldName = Fields[i].Name;
            if (Fields[i].DataType != typeof(string) || Fields[i].CurrentValue == null) continue;
			    
            // if the field names exists and there was no error, 
            // for this field, then set the value from the Model to ModelState
            if (modelState[fieldName]?.Errors.Count == 0 && modelState[fieldName]?.RawValue != null)
                modelState.SetModelValue(fieldName, new ValueProviderResult(new StringValues(Fields[i].CurrentValue as string), CultureInfo.InvariantCulture));
        }
    }


    public void Normalize()
    {
        // Trim and strip html tags if not allowed for all string fields
        for (var i=0; i < Fields.Count; i++)
        {
            if (Fields[i].DataType != typeof(string) || Fields[i].CurrentValue == null) continue;

            Fields[i].CurrentValue = NB.Tools.String.StringHelper.StripTags(Fields[i].CurrentValue as string ?? string.Empty).Trim();
        }

        if (string.IsNullOrEmpty(Nickname))
            Nickname = FirstName;

        if (MaxDistance.HasValue && MaxDistance.Value != 0)
        {
            if (MaxDistance < 50) MaxDistance = 50;
            if (MaxDistance > 6300) MaxDistance = 6300;
        }
        else
        {
            CountryId = null;
            ZipCode = City = Street = string.Empty;
            Longitude = Latitude = null;
            MaxDistance = null;
        }

        // if the email address is changed, it must be re-confirmed
        if (Fields[CalendarFields.Email.FieldIndex].IsChanged)
            ConfirmedOn = null;

        // editing before confirming is implicit confirmation
        if (EditMode == EditMode.Change && !ConfirmedOn.HasValue)
            ConfirmedOn = DateTime.Now;
    }

    public async Task <Models.Shared.ConfirmModel<InfoServiceEntity>> Save(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Guid) || IsNew)
        {
            IsNew = true;
            Guid = System.Guid.NewGuid().ToString("N");
            SubscribedOn = ModifiedOn = DateTime.Now;
            ConfirmedOn = UnSubscribedOn = null;
        }
        else
        {
            IsNew = false;
            if (!ConfirmedOn.HasValue) ConfirmedOn = DateTime.Now;
            UnSubscribedOn = null;
            ModifiedOn = DateTime.Now;
        }

        var confirmModel = new Models.Shared.ConfirmModel<InfoServiceEntity> { SaveSuccessful = false };

        try
        {
            // Id will be zero if Guid does not exist
            Id = _appDb!.InfoServiceRepository.GetIdForGuid(Guid);

            confirmModel.SaveSuccessful = await _appDb!.GenericRepository.Save(this, true, cancellationToken);
            confirmModel.Entity = this;
        }
        catch (Exception ex)
        {
            confirmModel.Exception = ex;
        }

        return confirmModel;
    }

    [Bind("TeamName, ClubName, Gender, Title, FirstName, LastName, Nickname, CountryId, ZipCode, City, Street, " +
          "MaxDistance, Email, Guid, IsAddressEntered, Captcha"
    )]
    public class InfoServiceMetadata
    {
        [DataType(DataType.EmailAddress)]
        [RegularExpression(
            @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$"
            , ErrorMessage = "E-Mail hat ungültiges Format")]
        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="E-Mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="Anrede")]
        public string Gender { get; set; } = string.Empty;

        [Display(Name="Titel")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="Vorname")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name="Ruf-/Spitzname")]
        public string Nickname { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name="Familienname")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name="Mannschaftsname")]
        public string TeamName { get; set; } = string.Empty;

        [Display(Name="Vereinsname")]
        public string ClubName { get; set; } = string.Empty;

        [ValidateAddressFields("CountryId, ZipCode, City, Street")]
        [Display(Name="Angaben für die Entfernungsberechnung zum Veranstaltungsort aktivieren")]
        public bool IsAddressEntered { get; set; }

        [Display(Name="Max. Entfernung")]
        public int? MaxDistance { get; set; }

        [Display(Name="Land")]
        public string CountryId { get; set; } = string.Empty;

        [Display(Name="Postleitzahl")]
        public string ZipCode { get; set; } = string.Empty;

        [Display(Name="Ort")]
        public string City { get; set; } = string.Empty;

        [Display(Name="Straße")]
        public string Street { get; set; } = string.Empty;

        [Display(Name = "Ergebnis der Rechenaufgabe im Bild")]
        [ValidationAttributes.ValidateCaptchaText]
        [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
        public string Captcha { get; set; } = string.Empty;
    }

    #region IValidatableObject Members

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(_appDb == null) return Enumerable.Empty<ValidationResult>();

        // Will be called only after individual fields are valid
        var errors = new List<ValidationResult>();

        var email = Email.Trim();

        var info = new InfoServiceEntity();
        ExistingEntryWithSameEmail = null;
			
        // if email found
        if (_appDb!.InfoServiceRepository.GetRegistrationByEmail(info, email))
        {
            // email address was found in a different record than the current one
            if (info.Guid != Guid)
            {
                errors.Add(new ValidationResult(string.Format("E-Mail '{0}' ist bereits registriert", email), new[] { Email }));

                // the controller will decide what to do now
                ExistingEntryWithSameEmail = info;
            }
        }
        return errors;
    }

    #endregion
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ValidateAddressFieldsAttribute : ValidationAttribute
{
    private readonly string[] _addressFieldNames;

    public ValidateAddressFieldsAttribute(string addressFieldNames)
    {
        if (addressFieldNames == null)
            throw new ArgumentNullException(nameof(addressFieldNames));

        _addressFieldNames = addressFieldNames.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult(validationContext + " ist erforderlich");

        if ((bool)value)
        {
            foreach (var addressFieldName in _addressFieldNames)
            {
                // get property of address field
                var property = validationContext.ObjectType.GetProperty(addressFieldName);

                if (property == null)
                    return new ValidationResult(string.Format("Unbekannte Eigenschaft: {0}", addressFieldName));

                // check types
                if (property.PropertyType != typeof(string))
                    return new ValidationResult(string.Format("Datentyp von Feld {0} muss 'string' sein", addressFieldName));

                // get the field value
                var field = (string?)property.GetValue(validationContext.ObjectInstance, null);

                if (field?.Trim().Length == 0)
                    return new ValidationResult("Felder für Entfernungsberechnung komplett füllen oder Entfernungs-Kontrollkästchen abwählen");
            }
        }

        return null;
    }
}
