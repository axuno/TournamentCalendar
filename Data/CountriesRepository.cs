using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;


namespace TournamentCalendar.Data;

public class CountriesRepository : GenericRepository
{
    public CountriesRepository(IDbContext dbContext) : base(dbContext) { }

    public virtual void GetCountriesList(EntityCollection<CountryEntity> countries, string[] forIds)
    {
        using var da = _dbContext.GetNewAdapter();

        var bucket = new RelationPredicateBucket();
        foreach (var id in forIds)
        {
            bucket.PredicateExpression.AddWithOr(CountryFields.Id == id);
        }

        da.FetchEntityCollection(countries, bucket);
    }
}
