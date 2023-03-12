using TournamentCalender.Data;

namespace TournamentCalendar.Data;

/// <summary>
/// Provides access to database repositories.
/// </summary>
public class AppDb : IAppDb
{
    /// <summary>
    /// CTOR.
    /// </summary>
    /// <param name="dbContext"></param>
    public AppDb(IDbContext dbContext)
    {
        DbContext = dbContext;
    }

    /// <summary>
    /// Provides database-specific settings.
    /// </summary>
    public virtual IDbContext DbContext { get; }

    public virtual GenericRepository GenericRepository => new(DbContext);
    public virtual CalendarRepository CalendarRepository => new(DbContext);
    public virtual CountriesRepository CountriesRepository => new(DbContext);
    public virtual InfoServiceRepository InfoServiceRepository => new(DbContext);
    public virtual SentNewsletterRepository SentNewsletterRepository => new(DbContext);
}
