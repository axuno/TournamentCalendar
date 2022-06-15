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

namespace TournamentCalendar.Models.InfoService
{
	public enum EditMode
	{
		New, Change
	}

	[ModelMetadataType(typeof (InfoServiceMetadata))]
	public class EditModel : InfoServiceEntity, IValidatableObject
	{
		private bool _isAddressEntered = true;

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

		public EditModel(string guid)
		{
			LoadData(guid);
			IsAddressEntered = true;
		}

		private bool LoadData(string guid)
		{
			return InfoServiceRepository.GetRegistrationByGuid(this, guid);
		}

		public bool TryRefetchEntity()
		{
			return string.IsNullOrWhiteSpace(Guid) || LoadData(Guid);
		}

		public async Task<bool> TryGetLongitudeLatitude(NB.Tools.GeoSpatial.GoogleConfig googleConfig)
		{
			if (Fields[InfoServiceFields.Street.FieldIndex].IsChanged || Fields[InfoServiceFields.ZipCode.FieldIndex].IsChanged ||
			    Fields[InfoServiceFields.City.FieldIndex].IsChanged)
			{
				try
				{
					// try to get longitude and latitude by Google Maps API
					string completeAddress = string.Join(", ", CountryId, ZipCode, City, Street);

				    NB.Tools.GeoSpatial.GoogleGeo.GoogleApiKey = googleConfig.ServiceApiKey;
                    NB.Tools.GeoSpatial.Location location = await NB.Tools.GeoSpatial.GoogleGeo.GetLocation(completeAddress);
					if (location != null && location.Latitude.Degrees > 1 && location.Longitude.Degrees > 1)
					{
						Longitude = location.Longitude.TotalDegrees;
						Latitude = location.Latitude.TotalDegrees;
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
					new NB.Tools.GeoSpatial.Location(new NB.Tools.GeoSpatial.Latitude(NB.Tools.GeoSpatial.Angle.FromDegrees(48.3666)),
													 new NB.Tools.GeoSpatial.Longitude(NB.Tools.GeoSpatial.Angle.FromDegrees(10.894103)));

				var userLoc =
					new NB.Tools.GeoSpatial.Location(
						new NB.Tools.GeoSpatial.Latitude(NB.Tools.GeoSpatial.Angle.FromDegrees(Latitude.Value)),
						new NB.Tools.GeoSpatial.Longitude(NB.Tools.GeoSpatial.Angle.FromDegrees(Longitude.Value)));

				return (int) (userLoc.Distance(augsburg) + 500)/1000;
			}
			return 0;
		}

		public IEnumerable<SelectListItem> GetGenderList()
		{
			return new List<SelectListItem>(3)
			    {
			       	new SelectListItem {Value = "", Text = "Bitte wählen"},
			       	// a value of string.Empty will cause a required validation error
			       	new SelectListItem {Value = "f", Text = "Frau"},
			       	new SelectListItem {Value = "m", Text = "Herr"}
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

		public InfoServiceEntity ExistingEntryWithSameEmail { get; private set; }

		public string Captcha { get; set; }

		public IEnumerable<SelectListItem> GetCountriesList()
		{
			var countryIds = new[] { "DE", "AT", "CH", "LI", "IT", "NL", "BE", "LU", "FR", "PL", "DK" };

			var countries = new EntityCollection<CountryEntity>();
			CountriesRepository.GetCountriesList(countries, countryIds);

			// add to countries list in the sequence of countryIds array
			return countryIds.Select(id => countries.Where(c => c.Id == id).First()).Select(
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
			for (int i = 0; i < Fields.Count; i++)
			{
				var fieldName = Fields[i].Name;
				if (Fields[i].DataType != typeof(string) || Fields[i].CurrentValue == null) continue;
			    
				// if the field names exists and there was no error, 
				// for this field, then set the value from the Model to ModelState
				if (modelState[fieldName] != null && modelState[fieldName].Errors.Count == 0 && modelState[fieldName].RawValue != null)
					modelState.SetModelValue(fieldName, new ValueProviderResult(new StringValues(Fields[i].CurrentValue as string), CultureInfo.InvariantCulture));
			}
		}


		public void Normalize()
		{
		    // Trim and strip html tags if not allowed for all string fields
			for (int i=0; i < Fields.Count; i++)
			{
				if (Fields[i].DataType != typeof(string) || Fields[i].CurrentValue == null) continue;

				Fields[i].CurrentValue = NB.Tools.String.StringHelper.StripTags(Fields[i].CurrentValue as string).Trim();
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

		public async Task <Models.Shared.ConfirmModel<InfoServiceEntity>> Save()
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
				Id = InfoServiceRepository.GetIdforGuid(Guid);

				confirmModel.SaveSuccessful = await InfoServiceRepository.Save(this, true);
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
			public string Email { get; set; }

			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[Display(Name="Anrede")]
			public string Gender { get; set; }

			[Display(Name="Titel")]
			public string Title { get; set; }

			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[Display(Name="Vorname")]
			public string FirstName { get; set; }

			[Display(Name="Ruf-/Spitzname")]
			public string Nickname { get; set; }

			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[Display(Name="Familienname")]
			public string LastName { get; set; }

			[Display(Name="Mannschaftsname")]
			public string TeamName { get; set; }

			[Display(Name="Vereinsname")]
			public string ClubName { get; set; }

			[ValidateAddressFields("CountryId, ZipCode, City, Street")]
			[Display(Name="Angaben für die Entfernungsberechnung zum Veranstaltungsort aktivieren")]
			public bool IsAddressEntered { get; set; }

			[Display(Name="Max. Entfernung")]
			public int? MaxDistance { get; set; }

			[Display(Name="Land")]
			public string CountryId { get; set; }

			[Display(Name="Postleitzahl")]
			public string ZipCode { get; set; }

			[Display(Name="Ort")]
			public string City { get; set; }

			[Display(Name="Straße")]
			public string Street { get; set; }

			[Display(Name = "Ergebnis der Rechenaufgabe im Bild")]
			[ValidationAttributes.ValidateCaptchaText]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            public string Captcha { get; set; }
		}

		#region IValidatableObject Members

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			// Will be called only after individual fields are valid
			var errors = new List<ValidationResult>();

			var email = Email.Trim();

			var info = new InfoServiceEntity();
			ExistingEntryWithSameEmail = null;
			
			// if email found
			if (InfoServiceRepository.GetRegistrationByEmail(info, email))
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
		private string[] _addressFieldNames;

		public ValidateAddressFieldsAttribute(string addressFieldNames)
		{
			if (addressFieldNames == null)
				throw new ArgumentNullException(nameof(addressFieldNames));

			_addressFieldNames = addressFieldNames.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
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
					var field = (string)property.GetValue(validationContext.ObjectInstance, null);

					if (field.Trim().Length == 0)
						return new ValidationResult("Felder für Entfernungsberechnung komplett füllen oder Entfernungs-Kontrollkästchen abwählen");
				}
			}

			return null;
		}
	}
}