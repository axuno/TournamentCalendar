using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SD.LLBLGen.Pro.LinqSupportClasses;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendarDAL.Linq;

namespace TournamentCalendar.Data;

public class CountriesRepository : GenericRepository
{
    /// <summary>
    /// Duration for caching query results.
    /// </summary>
    public readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    /// <summary>
    /// The tag for all calendar caches. Caches can be purged using this tag.
    /// </summary>
    public static readonly string CacheTag = "Countries";

    public CountriesRepository(IDbContext dbContext) : base(dbContext) { }

    public virtual async Task GetCountriesList(EntityCollection<CountryEntity> countries, string[] forIds, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);
        var allCountries = await (from ce in metaData.Country select ce)
            .CacheResultset(CacheDuration, CacheTag)
            .ExecuteAsync<EntityCollection<CountryEntity>>(cancellationToken);

        countries.Clear();
        countries.AddRange(allCountries.Where(ac => forIds.Contains(ac.Id)));
    }
}
