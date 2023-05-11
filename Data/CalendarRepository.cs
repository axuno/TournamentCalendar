using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendarDAL.Linq;

namespace TournamentCalendar.Data;

public class CalendarRepository : GenericRepository
{
    /// <summary>
    /// Duration for caching query results.
    /// </summary>
    public readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(60);

    /// <summary>
    /// The tag for all calendar caches. Caches can be purged using this tag.
    /// </summary>
    public static readonly string CacheTag = "Calendar";

    public CalendarRepository(IDbContext dbContext) : base(dbContext) { }

    public virtual bool GetTournamentByGuid(CalendarEntity entity, string guid, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        return da.FetchEntityUsingUniqueConstraint(entity, new PredicateExpression(CalendarFields.Guid == guid));
    }

    public virtual async Task<long> GetIdForGuid(string guid, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);

        // If Guid does not exist, Id will be zero:
        return (await (from t in metaData.Calendar where t.Guid == guid select t.Id).ExecuteAsync<ICollection<long>>(cancellationToken)).FirstOrDefault();
    }

    public virtual async Task GetAllActiveTournaments(EntityCollection<CalendarEntity> tournaments, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        tournaments.Clear();
        var at = await GetActiveTournaments(cancellationToken);
        tournaments.AddRange(at);
    }

    /// <summary>
    /// Gets the calendar entries for tournaments starting in the future,
    /// and which are approved, but may have been deleted (e.g. because they are fully booked already).
    /// This is to compare local calendar entries witch tourneys collected from the web.
    /// </summary>
    /// <param name="afterThisDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<EntityCollection<CalendarEntity>> GetActiveOrDeletedTournaments(DateTime afterThisDate, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);
        var result = from tc in metaData.Calendar
            where tc.DateFrom >= afterThisDate.Date && tc.ApprovedOn != null
            select tc;
        var coll = new EntityCollection<CalendarEntity>();
        coll.AddRange(await result.ExecuteAsync<ICollection<CalendarEntity>>(cancellationToken));
        return coll;
    }

    public virtual bool FetchEntity<T>(T entity, PredicateExpression predicateExpression) where T : EntityBase2, new()
    {
        using var da = _dbContext.GetNewAdapter();
        return da.FetchEntityUsingUniqueConstraint(entity, predicateExpression);
    }

    public virtual async Task GetActiveTournaments(EntityCollection<CalendarEntity> tournaments, string guid, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        tournaments.Clear();
        var at = (await GetActiveTournaments(cancellationToken)).Where(t => t.Guid == guid);
        tournaments.AddRange(at);
    }

    public virtual async Task GetActiveTournaments(EntityCollection<CalendarEntity> tournaments, long id, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        tournaments.Clear();
        var at = (await GetActiveTournaments(cancellationToken)).Where(t => t.Id == id);
        tournaments.AddRange(at);
    }

    public virtual async Task<bool> SaveEntity<T>(T entity, bool refetchAfterSave) where T : EntityBase2, new()
    {
        using var da = _dbContext.GetNewAdapter();
        var result = await da.SaveEntityAsync(entity, refetchAfterSave);
        PurgeCalendarCaches();
        return result;
    }

    public virtual async Task<List<CalendarEntity>> GetActiveTournaments(DateTime sinceDate, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        return (await GetActiveTournaments(cancellationToken)).Where(t => t.ModifiedOn >= sinceDate).ToList();
    }

    private async Task<List<CalendarEntity>> GetActiveTournaments(CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);
        var result = await (from tc in metaData.Calendar
            where tc.DateFrom > DateTime.Today.Date && tc.ApprovedOn != null && tc.DeletedOn == null
            orderby tc.DateFrom ascending
            select tc).CacheResultset(CacheDuration, CacheTag).ExecuteAsync<EntityCollection<CalendarEntity>>(cancellationToken);
        return result.ToList();
    }

    /// <summary>
    /// Uses DateFrom, CountryId, PostalCode, Surface, NumPlayersMale, NumPlayersFemale and ApprovedOn.HasValue
    /// to compare with an existing tournament calendar entry.
    /// </summary>
    /// <param name="entry">TournamentCalendarEntity</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the Id of a possible duplicate if found, else Null.</returns>
    public virtual async Task<CalendarEntity?> GetPossibleDuplicate(CalendarEntity entry, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);

        // check for duplicate criteria an existing and approved other tournament calender entries
        if (entry.IsNew)
        {
            return (await (from tc in metaData.Calendar
                    where
                        (tc.DateFrom.Date == entry.DateFrom.Date && tc.CountryId == entry.CountryId &&
                         tc.PostalCode == entry.PostalCode &&
                         tc.Surface == entry.Surface && tc.NumPlayersMale == entry.NumPlayersMale &&
                         tc.NumPlayersFemale == entry.NumPlayersFemale && tc.ApprovedOn.HasValue &&
                         !tc.DeletedOn.HasValue)
                    select tc).WithPath(
                    new PathEdge<SurfaceEntity>(CalendarEntity.PrefetchPathTournamentSurface))
                .ExecuteAsync<ICollection<CalendarEntity>>(cancellationToken)).FirstOrDefault();
        }

        return (await (from tc in metaData.Calendar
                where
                    (tc.DateFrom.Date == entry.DateFrom.Date && tc.CountryId == entry.CountryId &&
                     tc.PostalCode == entry.PostalCode &&
                     tc.Surface == entry.Surface && tc.NumPlayersMale == entry.NumPlayersMale &&
                     tc.NumPlayersFemale == entry.NumPlayersFemale && tc.ApprovedOn.HasValue &&
                     tc.Guid != entry.Guid)
                select tc).WithPath(
                new PathEdge<SurfaceEntity>(CalendarEntity.PrefetchPathTournamentSurface))
            .ExecuteAsync<ICollection<CalendarEntity>>(cancellationToken)).FirstOrDefault();
    }

    public virtual async Task GetTournamentRelationshipEntities(EntityCollection<SurfaceEntity> surface, EntityCollection<PlayingAbilityEntity> ability, CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        await da.FetchEntityCollectionAsync(new QueryParameters { CollectionToFetch = surface, CacheResultset = true, CacheDuration = CacheDuration, CacheTag = CacheTag}, cancellationToken);
        await da.FetchEntityCollectionAsync(new QueryParameters { CollectionToFetch = ability, CacheResultset = true, CacheDuration = CacheDuration, CacheTag = CacheTag}, cancellationToken);
    }

    /// <summary>
    /// Remove tagged result sets from the <see cref="CacheController"/>.
    /// </summary>
    private static void PurgeCalendarCaches()
    {
        CacheController.PurgeResultsets(CacheTag);
    }
}
