using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TournamentCalendar.Data;
using TournamentCalendar.Library;
using TournamentCalendar.Services;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;

namespace TournamentCalendar.Models.GeoLocation;

public class EditModel : IValidatableObject
{
    private IAppDb? _appDb;
    private readonly string[] _countryIds = new[] { "DE", "AT", "CH", "LI", "IT", "NL", "BE", "LU", "FR", "PL", "DK", "CZ", "SK" };

    [Display(Name = "Land")]
    public string? CountryId { get; set; }

    [Display(Name = "Postleitzahl")]
    public string? ZipCode { get; set; }

    [Display(Name = "Ort")]
    public string? City { get; set; }

    [Display(Name = "Straße")]
    public string? Street { get; set; }

    public void SetAppDb(IAppDb appDb)
    {
        _appDb = appDb;
    }

    public async Task<IEnumerable<SelectListItem>> GetCountriesList()
    {
        var countries = new EntityCollection<CountryEntity>();
        await _appDb!.CountriesRepository.GetCountriesList(countries, _countryIds, CancellationToken.None);

        // add to countries list in the sequence of countryIds array
        return _countryIds.Select(id => countries.First(c => c.Id == id)).Select(
            item => new SelectListItem { Value = item.Id, Text = item.Name }).ToList();
    }

    public async Task<UserLocation> TryGetLongitudeLatitude(GoogleConfiguration googleConfig)
    {
        try
        {
            // try to get longitude and latitude by Google Maps API
            var completeAddress = string.Join(", ", ZipCode, City, Street);
            CountryId ??= "DE";

            var location = await Axuno.Tools.GeoSpatial.GoogleGeo.GetLocation(CountryId, completeAddress,
                googleConfig.ServiceApiKey, TimeSpan.FromSeconds(15));
            if (location.GeoLocation.Latitude != null && location.GeoLocation.Latitude.Degrees > 1 &&
                location.GeoLocation.Longitude?.Degrees > 1)
            {
                return new UserLocation(location.GeoLocation.Latitude.TotalDegrees,
                    location.GeoLocation.Longitude.TotalDegrees);
            }
        }
        catch (Exception)
        {
            // ignored
        }

        return new UserLocation(null, null);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Will be called only after individual fields are valid
        var errors = new List<ValidationResult>();

        if (CountryId == null || !_countryIds.ToList().Contains(CountryId))
            errors.Add(new ValidationResult("'Land' aus der Liste ist erforderlich", new[] {nameof(CountryId)}));

        if (string.IsNullOrEmpty(ZipCode) && string.IsNullOrEmpty(City))
            errors.Add(new ValidationResult("'Postleitzahl' oder 'Ort' sind erforderlich", new[] { nameof(ZipCode), nameof(City) }));

        return errors;
    }
}
