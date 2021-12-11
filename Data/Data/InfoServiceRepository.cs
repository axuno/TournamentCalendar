using System;
using System.Linq;
using System.Threading.Tasks;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendarDAL.Linq;

namespace TournamentCalendar.Data
{
	public class InfoServiceRepository : GenericRepository
    {
		public static bool GetRegistrationByGuid(InfoServiceEntity entity, string guid)
		{
			return GetRegistration(entity, new PredicateExpression(InfoServiceFields.Guid == guid));
		}

		public static bool GetRegistrationByEmail(InfoServiceEntity entity, string email)
		{
			return GetRegistration(entity, new PredicateExpression(InfoServiceFields.Email == email));
		}

		public static bool GetRegistration(InfoServiceEntity entity, PredicateExpression filter)
		{
			using (var da = Connecter.GetNewAdapter())
			{
				var success = da.FetchEntityUsingUniqueConstraint(entity, filter);
				da.CloseConnection();
				return success;
			}
		}

		public static int GetIdforGuid(string guid)
		{
			using (var da = Connecter.GetNewAdapter())
			{
				var metaData = new LinqMetaData(da);

				// If Guid does not exist, Id will be zero:
				return (from t in metaData.InfoService where t.Guid == guid select t.Id).First();
			}
		}

        public static async Task <EntityCollection<InfoServiceEntity>> GetActiveSubscribers()
        {
            using (var da = Connecter.GetNewAdapter())
            {
                var metaData = new LinqMetaData(da);
                return await (from subs in metaData.InfoService
                    where !subs.UnSubscribedOn.HasValue select subs).ExecuteAsync<EntityCollection<InfoServiceEntity>>();
            }
        }
    }
}
