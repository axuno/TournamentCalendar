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
    public static bool GetTournamentByGuid(CalendarEntity entity, string guid)
    {
        using var da = Connecter.GetNewAdapter();
        return da.FetchEntityUsingUniqueConstraint(entity, new PredicateExpression(CalendarFields.Guid == guid));
    }

    public static async Task<long> GetIdForGuid(string guid)
    {
        using var da = Connecter.GetNewAdapter();
        var metaData = new LinqMetaData(da);

        // If Guid does not exist, Id will be zero:
        return (await (from t in metaData.Calendar where t.Guid == guid select t.Id).ExecuteAsync<ICollection<long>>()).FirstOrDefault();
    }

    public static async Task GetAllActiveTournaments(EntityCollection<CalendarEntity> tournaments)
    {
        using var da = Connecter.GetNewAdapter();
        tournaments.Clear();
        var at = await GetActiveTournaments(da).ExecuteAsync<ICollection<CalendarEntity>>();
        tournaments.AddRange(at);
    }

    public static bool FetchEntity<T>(T entity, PredicateExpression predicateExpression) where T : EntityBase2, new()
    {
        using var da = Connecter.GetNewAdapter();
        return da.FetchEntityUsingUniqueConstraint(entity, predicateExpression);
    }

    public static async Task GetActiveTournaments(EntityCollection<CalendarEntity> tournaments, string guid)
    {
        using var da = Connecter.GetNewAdapter();
        tournaments.Clear();
        var at = await GetActiveTournaments(da).Where(t => t.Guid == guid).ExecuteAsync<ICollection<CalendarEntity>>();
        tournaments.AddRange(at);
    }

    public static async Task GetActiveTournaments(EntityCollection<CalendarEntity> tournaments, long id)
    {
        using var da = Connecter.GetNewAdapter();
        tournaments.Clear();
        var at = await GetActiveTournaments(da).Where(t => t.Id == id).ExecuteAsync<ICollection<CalendarEntity>>();
        tournaments.AddRange(at);
    }

    public static async Task<bool> SaveEntity<T>(T entity, bool refetchAfterSave) where T : EntityBase2, new()
    {
        using var da = Connecter.GetNewAdapter();
        return await da.SaveEntityAsync(entity, refetchAfterSave);
    }

    public static async Task<EntityCollection<CalendarEntity>> GetActiveTournaments(DateTime sinceDate)
    {
        using var da = Connecter.GetNewAdapter();
        return await GetActiveTournaments(da).Where(t => t.ModifiedOn >= sinceDate).ExecuteAsync<EntityCollection<CalendarEntity>>();
    }

    private static IOrderedQueryable<CalendarEntity> GetActiveTournaments(IDataAccessAdapter da)
    {
        var metaData = new LinqMetaData(da);
        var result = from tc in metaData.Calendar
            where tc.DateFrom > DateTime.Today.Date && tc.ApprovedOn != null && tc.DeletedOn == null
            orderby tc.DateFrom ascending
            select tc;
        return result;
    }

    /// <summary>
    /// Uses DateFrom, CountryId, PostalCode, Surface, NumPlayersMale, NumPlayersFemale and ApprovedOn.HasValue
    /// to compare with an existing tournament calendar entry.
    /// </summary>
    /// <param name="entry">TournamentCalendarEntity</param>
    /// <returns>Returns the Id of a possible duplicate if found, else Null.</returns>
    public static async Task<CalendarEntity?> GetPossibleDuplicate(CalendarEntity entry)
    {
        using var da = Connecter.GetNewAdapter();
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
                .ExecuteAsync<ICollection<CalendarEntity>>()).FirstOrDefault();
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
            .ExecuteAsync<ICollection<CalendarEntity>>()).FirstOrDefault();
    }

    public static async Task GetTournamentRelationshipEntities(EntityCollection<SurfaceEntity> surface, EntityCollection<PlayingAbilityEntity> ability)
    {
        using var da = Connecter.GetNewAdapter();
        await da.FetchEntityCollectionAsync(new QueryParameters { CollectionToFetch = surface }, CancellationToken.None);
        await da.FetchEntityCollectionAsync(new QueryParameters { CollectionToFetch = ability }, CancellationToken.None);
        da.CloseConnection();
    }
}