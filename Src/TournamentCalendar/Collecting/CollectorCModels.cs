using System.Text.Json.Serialization;

namespace TournamentCalendar.Collecting;

/// <summary>
/// Represents a single tournament entry as returned by the volleyballfreakportal.de
/// "/api/tournaments/search" endpoint.
/// </summary>
public class CollectorCTournament
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("organizer")]
    public string? Organizer { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonPropertyName("street")]
    public string? Street { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    [JsonPropertyName("startPlaces")]
    public int? StartPlaces { get; set; }

    [JsonPropertyName("participationFee")]
    public double? ParticipationFee { get; set; }

    [JsonPropertyName("durationInDays")]
    public int? DurationInDays { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("availablePlaces")]
    public int? AvailablePlaces { get; set; }

    [JsonPropertyName("registrationDeadline")]
    public DateTime? RegistrationDeadline { get; set; }

    [JsonPropertyName("info")]
    public string? Info { get; set; }

    [JsonPropertyName("pdfFile")]
    public string? PdfFile { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// The paged "content" wrapper returned by the search endpoint.
/// </summary>
public class CollectorCSearchResult
{
    [JsonPropertyName("content")]
    public List<CollectorCTournament> Content { get; set; } = [];

    [JsonPropertyName("totalElements")]
    public int TotalElements { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("number")]
    public int Number { get; set; }
}
