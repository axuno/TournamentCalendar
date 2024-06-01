using TournamentCalender.Data;

namespace TournamentCalendar.Data;

/// <summary>
/// Interface for accessing the repositories.
/// </summary>
public interface IAppDb
{
    /// <summary>
    /// The <see cref="IDbContext"/> instance to be used to access the repositories.
    /// </summary>
    IDbContext DbContext { get; }

    GenericRepository GenericRepository => new(DbContext);
    CalendarRepository CalendarRepository => new(DbContext);
    CountriesRepository CountriesRepository => new(DbContext);
    InfoServiceRepository InfoServiceRepository => new(DbContext);
    SentNewsletterRepository SentNewsletterRepository => new(DbContext);
}
