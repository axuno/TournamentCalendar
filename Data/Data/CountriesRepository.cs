using System.Linq;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendarDAL.Linq;


namespace TournamentCalendar.Data
{
	public class CountriesRepository : GenericRepository
    {
		public static void GetCountriesList(EntityCollection<CountryEntity> countries, string[] forIds)
		{
            using (var da = Connecter.GetNewAdapter())
			{
				var bucket = new RelationPredicateBucket();
				foreach (var id in forIds)
				{
					bucket.PredicateExpression.AddWithOr(CountryFields.Id == id);
				}

				da.FetchEntityCollection(countries, bucket);
				da.CloseConnection();
			}
		}
    }
}
