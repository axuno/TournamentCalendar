using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
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

namespace TournamentCalendar.Models.Calendar
{
    public enum EditMode
	{
		New, Change
	}

	[ModelMetadataType(typeof (TournamentCalendarMetadata))]
	public class EditModel : CalendarEntity, IValidatableObject
	{
		private readonly EntityCollection<SurfaceEntity> _surfaces = new();
		private readonly EntityCollection<PlayingAbilityEntity> _playingAbilities = new();

		public EditModel()
		{
			base.DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0).AddDays(2);
			base.DateTo = base.DateFrom.AddHours(9);
			base.ClosingDate = base.DateFrom.Date.AddDays(-1);
			base.NumPlayersFemale = base.NumPlayersMale = 3;
			base.EntryFee = base.Bond = 0;
			base.NumOfTeams = 0;
			base.PlayingAbilityFrom = base.PlayingAbilityTo = 1;
			base.Surface = 1;
			base.ApprovedOn = null;
			base.DeletedOn = null;
			base.Special = string.Empty;
			base.IsNew = true;
        }

	    public async Task<EditModel> Initialize()
	    {
	        await CalendarRepository.GetTournamentRelationshipEntities(_surfaces, _playingAbilities);
	        return this;
	    }

		public bool LoadTournament(string guid)
		{
			if (!CalendarRepository.GetTournamentByGuid(this, guid)) return false;

            Guid = guid;
			return true;
		}

		public bool TryRefetchEntity()
		{
			return string.IsNullOrWhiteSpace(Guid) || LoadTournament(Guid);
		}

		public async Task<bool> TryGetLongitudeLatitude(GoogleConfiguration googleConfig)
		{
			if (Fields[CalendarFields.Street.FieldIndex].IsChanged || Fields[CalendarFields.PostalCode.FieldIndex].IsChanged || Fields[CalendarFields.City.FieldIndex].IsChanged)
			{
				try
				{
					// try to get longitude and latitude by Google Maps API
					var completeAddress = string.Join(", ", PostalCode, City, Street);
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

		public EditMode EditMode
		{ get; set; }


	    /// <summary>
		/// DateFrom (date part). The date and time parts are taken from 2 different text boxes
		/// </summary>
		public string DateFromText
		{
			get => DateFrom.ToString("dd.MM.yyyy");
		    set
			{
			    if (DateTime.TryParse(value, out var date))
			        DateFrom = date.Date + DateFrom.TimeOfDay;
			    else
			    {
			        throw new Exception("Datum ist ungültig");
			    }

			    if (TimeSpan.TryParse(TimeTo, out var time))
			        DateFrom = DateFrom.Date + time;
            }
		}

		/// <summary>
		/// DateTo (date part). The date and time parts are taken from 2 different text boxes
		/// </summary>
		public string DateToText
		{
			get => DateTo.ToString("dd.MM.yyyy");
		    set
			{
			    if (DateTime.TryParse(value, out var date))
			        DateTo = date.Date + DateTo.TimeOfDay;
			    else
			    {
			        throw new Exception("Date is invalid");
			    }

                if (TimeSpan.TryParse(TimeTo, out var time))
                    DateTo = DateTo.Date + time;
            }
		}

		/// <summary>
		/// TimeFrom (time part). The date and time parts are taken from 2 different text boxes
		/// </summary>
		public string TimeFrom
		{
            get => DateFrom.ToString("HH:mm");
		    set
            {
                if (TimeSpan.TryParse(value, out var time))
                    DateFrom = DateFrom.Date + time;
                else
                {
                    throw new Exception("Date is invalid");
                }
            }
		}

		/// <summary>
		/// TimeTo (time part). The date and time parts are taken from 2 different text boxes
		/// </summary>
		public string TimeTo
		{
			get => DateTo.ToString("HH:mm");
		    set
			{
                if (TimeSpan.TryParse(value, out var time))
                    DateTo = DateTo.Date + time;
            }
		}

	    public string ClosingDateText
	    {
	        get => ClosingDate.ToString("dd.MM.yyyy");
	        set
	        {
	            if (DateTime.TryParse(value, out var date))
	                ClosingDate = date.Date;
	            else
	            {
	                throw new Exception("Datum ist ungültig");
	            }
	        }
	    }

        public int MinMaxFemale
		{
			get => IsMinPlayersFemale ? 1 : IsMinPlayersMale ? 2 : 0;
		    set
			{
				switch (value)
				{
					case 0:
						IsMinPlayersFemale = false;
						IsMinPlayersMale = false;
						break;
					case 1:
						IsMinPlayersFemale = true;
						IsMinPlayersMale = false;
						break;
					case 2:
						IsMinPlayersFemale = false;
						IsMinPlayersMale = true;
						break;
				}
			}
		}

		public int MinMaxMale
		{
			get => IsMinPlayersMale ? 1 : IsMinPlayersFemale ? 2 : 0;
		    set
			{
				switch (value)
				{
					case 0:
						IsMinPlayersFemale = false;
						IsMinPlayersMale = false;
						break;
					case 1:
						IsMinPlayersFemale = false;
						IsMinPlayersMale = true;
						break;
					case 2:
						IsMinPlayersFemale = true;
						IsMinPlayersMale = false;
						break;
				}
			}
		}

		/// <summary>
		/// The question to the user is asked in a way, that clicking the checkbox
		/// means to toggle the approval status:
		/// if (Approved) { "Click to hide the tournament" } else { "Click to show the tournament" }
		/// </summary>
		public bool Approved
		{
			get => base.ApprovedOn.HasValue && !base.DeletedOn.HasValue;
		    set
			{
				if (!base.ApprovedOn.HasValue) base.ApprovedOn = DateTime.Now;

                if (base.IsNew)
                {
                    base.DeletedOn = null;
                }
                else
                {
                    if (value)
                    {
                        base.DeletedOn = (!base.DeletedOn.HasValue ? DateTime.Now : (DateTime?) null);
                    }
                }
            }
		}

		public string Captcha { get; set; } = string.Empty;

		public IEnumerable<SelectListItem> GetMinMaxPlayerList()
		{
			return new List<SelectListItem>(3)
				    {
						new() {Value = "0", Text = "genau"},
				       	new() {Value = "1", Text = "mindestens"},
				       	new() {Value = "2", Text = "maximal"}
				    };
		}

		public IEnumerable<SelectListItem> GetCountriesList()
		{
			var countryIds = new[] { "DE", "AT", "CH", "LI", "IT", "NL", "BE", "LU", "FR", "PL", "DK", "CZ", "SK" };

            EntityCollection<CountryEntity> countries = new();
			CountriesRepository.GetCountriesList(countries, countryIds);

			// add to countries list in the sequence of countryIds array
			return countryIds.Select(id => countries.First(c => c.Id == id)).Select(
					item => new SelectListItem {Value = item.Id, Text = item.Name}).ToList();
		}

		public IEnumerable<SelectListItem> GetPlayingAbilityList()
		{
			return _playingAbilities.Select(pa => new SelectListItem() {Value = pa.Strength.ToString(), Text = pa.Description}).ToList();
		}

		public IEnumerable<SelectListItem> GetSurfaceList()
		{
			return _surfaces.Select(pa => new SelectListItem() { Value = pa.Id.ToString(), Text = pa.Description }).ToList();
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
			// if we don't know when to start, we also don't know when to end
			var timeUnknown = new TimeSpan(0, 0, 0);
			if (DateFrom.TimeOfDay == timeUnknown)
				DateTo = DateTo.Date + timeUnknown;

			// swap from/to date if necessary
			if (DateTo.TimeOfDay != timeUnknown && DateFrom > DateTo)
			{
				(DateFrom, DateTo) = (DateTo, DateFrom);
            }

			// index is in ascending order - swap from/to if necessary
			var playingAbility = GetPlayingAbilityList().Select(pa => pa.Text).ToArray();
			if (Array.IndexOf(playingAbility, PlayingAbilityFrom) > Array.IndexOf(playingAbility, PlayingAbilityTo))
			{
				(PlayingAbilityFrom, PlayingAbilityTo) = (PlayingAbilityTo, PlayingAbilityFrom);
            }

			// if male or female numbers are minimum, the other one is no minimum
			if (IsMinPlayersFemale)
				IsMinPlayersMale = false;

			if (IsMinPlayersMale)
				IsMinPlayersFemale = false;

			if (!string.IsNullOrEmpty(Website) && !Website.StartsWith("http://") && !Website.StartsWith("https://"))
				Website = "http://" + Website;

			
			// Trim and strip html tags for all string fields
			for (var i=0; i < Fields.Count; i++)
			{
				if (Fields[i].DataType != typeof(string) || Fields[i].CurrentValue == null) continue;

				Fields[i].CurrentValue = NB.Tools.String.StringHelper.StripTags(Fields[i].CurrentValue as string ?? string.Empty).Trim(); // strip HTML tags
			}
		}

		public CalendarEntity? PossibleDuplicateFound { get; private set; }

		public async Task<CalendarEntity?> GetPossibleDuplicate()
		{
			// It is expected that the model is normalized before calling this method.
			return await CalendarRepository.GetPossibleDuplicate(this);
		}


		public async Task<Shared.ConfirmModel<CalendarEntity>> Save()
		{
			if (string.IsNullOrWhiteSpace(Guid) || IsNew)
			{
				IsNew = true;
				Guid = System.Guid.NewGuid().ToString("N");
				CreatedOn = ModifiedOn = DateTime.Now;
			}
			else
			{
				IsNew = false;
				if (IsDirty)
					ModifiedOn = DateTime.Now;
			}

			var confirmModel = new Models.Shared.ConfirmModel<CalendarEntity> { SaveSuccessful = false, Entity = this};

			try
			{
				// Id will be zero if the Guid does not exist:
				Id = await CalendarRepository.GetIdForGuid(Guid);

				confirmModel.SaveSuccessful = await GenericRepository.Save(this, true);
				confirmModel.Entity = this;
			}
			catch (Exception ex)
			{
				confirmModel.Exception = ex;
			}

			return confirmModel;
		}

		[Bind("Guid, Special, TournamentName, DateFromText, DateToText, TimeFrom, TimeTo, ClosingDateText, Venue, CountryId, PostalCode, City, Street, " +
				"Longitude, Latitude, NumOfTeams, NumPlayersMale, MinMaxFemale, NumPlayersFemale, MinMaxMale, " +
				"Surface, PlayingAbilityFrom, PlayingAbilityTo, EntryFee, Bond, Info, Organizer, ContactAddress, " +
				"Email, Website, PostedByName, PostedByEmail, PostedByPassword, Approved, Captcha"
			)]
		public class TournamentCalendarMetadata
        {
			[HiddenInput]
			public string? Guid { get; set; }

		    [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[Display(Name="Turniername")]
			public string? TournamentName { get; set; }

			[Display(Name="Spezialangaben")]
			public string? Special { get; set; }

			[ValidateDate]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            [Display(Name="Datum von")]
			public string? DateFromText { get; set; }

		    [ValidateDate]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            [Display(Name="Datum bis")]
			public string? DateToText { get; set; }

		    [RegularExpression("([0-1]{0,1}[0-9]|[2][0-3]):([0-5][0-9])", ErrorMessage = "'{0}' hat ungültiges Format")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[Display(Name="Uhrzeit von")]
			public string? TimeFrom { get; set; }

		    [RegularExpression("([0-1]{0,1}[0-9]|[2][0-3]):([0-5][0-9])", ErrorMessage = "'{0}' hat ungültiges Format")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[Display(Name="Uhrzeit bis")]
			public string? TimeTo { get; set; }

			[ValidateClosingDate("DateFromText")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[Display(Name="Anmeldeschluss")]
			public string? ClosingDateText { get; set; }

			[Display(Name="Anzahl Damen")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            [Range(0, 6, ErrorMessage = "'{0}' zwischen {1} und {2}")]
			public int NumPlayersFemale { get; set; }

			[Display(Name="Anzahl Herren")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            [Range(0, 6, ErrorMessage = "'{0}' zwischen {1} und {2}")]
			public int NumPlayersMale { get; set; }

			[Display(Name="Anzahl Teams")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[Range(0, 998, ErrorMessage = "'{0}' zwischen {1} und {2}")]
			public int NumOfTeams { get; set; }

			[Display(Name = "Spielstärke von")]
			public long PlayingAbilityFrom { get; set; }

			[Display(Name = "Spielstärke bis")]
			public long PlayingAbilityTo { get; set; }

			[Display(Name = "Belag")]
			public long Surface { get; set; }

			[Display(Name="Name des Austragungsorts (Halle bzw. Platz)")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			public string? Venue { get; set; }

			[Display(Name="Straße")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			public string? Street { get; set; }

			[Display(Name = "Land")]
			public string? CountryId { get; set; }

			[Display(Name="Postleitzahl")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			public string? PostalCode { get; set; }

			[Display(Name="Ort")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			public string? City { get; set; }

			[Display(Name="Veranstalter")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			public string? Organizer { get; set; }

			[Display(Name = "Ansprechpartner")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			[MaxLength(1024, ErrorMessage = "{0}: Max. {1} Zeichen")]
			public string? ContactAddress { get; set; }

			[Display(Name = "E-Mail Ansprechpartner")]
            [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceName = "EmailAddressInvalid", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			public string? Email { get; set; }

			[Display(Name = "Web-Adresse")]
			[RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", ErrorMessage = "'{0}' hat ungültiges Format")]
            public string? Website { get; set; }

			[Display(Name="Startgebühr")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00}")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            [Range(0.00, 998.00, ErrorMessage = "'{0}' zwischen {1} und {2}")]
			public decimal EntryFee { get; set; }

			[Display(Name= "Kaution")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00}")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            [Range(0.00, 998.00, ErrorMessage = "'{0}' zwischen {1} und {2}")]
			public decimal Bond { get; set; }

			[Display(Name = "'Weitere Infos'")]
			[MaxLength(1024, ErrorMessage = "{0}: Max. {1} Zeichen")]
			public string? Info { get; set; }

			[Display(Name="'Gemeldet von' Name")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			public string? PostedByName { get; set; }

			[Display(Name="'Gemeldet von' E-Mail")]
			[Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            [EmailAddress(ErrorMessageResourceName = "EmailAddressInvalid", ErrorMessageResourceType = typeof(DataAnnotationResource))]
			public string? PostedByEmail { get; set; }

            [Display(Name = "Ergebnis der Rechenaufgabe im Bild")]
            [ValidationAttributes.ValidateCaptchaText]
            [Required(ErrorMessageResourceName = "PropertyValueRequired", ErrorMessageResourceType = typeof(DataAnnotationResource))]
            public string? Captcha { get; set; }
        }

		#region IValidatableObject Members

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var errors = new List<ValidationResult>();

			if (NumPlayersMale + NumPlayersFemale > 6)
			{
				errors.Add(new ValidationResult("Zu viele Spieler", new []{nameof(NumPlayersMale), nameof(NumPlayersFemale)}));
			}

			if (NumPlayersMale + NumPlayersFemale < 2)
			{
				errors.Add(new ValidationResult("Zu wenige Spieler", new[] { nameof(NumPlayersMale), nameof(NumPlayersFemale) }));
			}

			return errors;
		}

		#endregion
	}

	
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ValidateDateAttribute : RangeAttribute
	{
		/*
        // Range attribute requires the following javascript extension
        $.validator.methods.range = function(value, element, param) {
            if ($(element).attr('data-input-type') === 'date') {
                var min = $(element).attr('data-val-range-min');
                var max = $(element).attr('data-val-range-max');
                var date = moment(value, 'DD.MM.YYYY');
                var minDate = new Date(min).getTime();
                var maxDate = new Date(max).getTime();
                return this.optional(element) || (date >= minDate && date <= maxDate);
            }		 
		}
		*/
		public ValidateDateAttribute()
			: base(typeof(DateTime), DateTime.Now.Date.AddDays(2).ToShortDateString(), DateTime.Now.Date.AddYears(1).ToShortDateString())
		{
			ErrorMessage = "'{0}' muss mind. 2 Tage, max. 1 Jahr in der Zukunft liegen";
		}
	}

    /*[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && DateTime.Now.AddDays(2).CompareTo(date) >= 0 && DateTime.Now.AddYears(1).CompareTo(date) <= 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"'{validationContext.DisplayName}' muss mind. 2 Tage, max. 1 Jahr in der Zukunft liegen",
                new[] { validationContext.MemberName });
        }
    }*/

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ValidateClosingDateAttribute : ValidationAttribute
	{
		private readonly string _otherDateFieldname;

		public ValidateClosingDateAttribute(string otherDate)
		{
            _otherDateFieldname = otherDate ?? throw new ArgumentNullException(nameof(otherDate));
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value == null)
				return new ValidationResult(validationContext + " ist erforderlich");

		    if (!DateTime.TryParse(value as string, out var closingDate))
		    {
		        return new ValidationResult(validationContext + " hat ungültiges Format");
            }
			
			// then the other property
			var property = validationContext.ObjectType.GetProperty(_otherDateFieldname);

			// check it is not null
			if (property == null)
				return new ValidationResult(string.Format("Unknown property: {0}", _otherDateFieldname));

			// check types
			if (((EditModel)validationContext.ObjectInstance).ClosingDateText.GetType() != property.PropertyType)
				return new ValidationResult(string.Format("The types of {0} and {1} must be the same", validationContext.DisplayName, _otherDateFieldname));

            // get the other value
		    if (!DateTime.TryParse(property.GetValue(validationContext.ObjectInstance, null) as string, out var otherDate))
		    {
		        return new ValidationResult($"'{_otherDateFieldname}' hat ungültiges Format");
		    }

			return closingDate >= DateTime.Now.Date.AddDays(1) && closingDate.Date < otherDate.Date
			       	? null
					: new ValidationResult(string.Format(ErrorMessage = "'{0}' muss vor Turnierbeginn und mind. 1 Tag in der Zukunft liegen", validationContext.DisplayName));
		}
	}
}