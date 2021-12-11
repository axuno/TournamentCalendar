using System;
using System.Threading.Tasks;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace TournamentCalendar.Data
{
	public class GenericRepository
	{
		public static async Task<bool> Save<T>(T registration, bool refetchAfterSave) where T: IEntity2
		{
			using (var da = Connecter.GetNewAdapter())
			{
				var success = await da.SaveEntityAsync(registration, refetchAfterSave);
				da.CloseConnection();
				return success;
			}
		}

        public static bool UpdateDateIfNotSet<T>(T entity, IPredicate filter, EntityField2 field, DateTime date , out bool found) where T: IEntity2, new()
		{
			using (var da = Connecter.GetNewAdapter())
			{
				if (!(found = da.FetchEntityUsingUniqueConstraint(entity, new PredicateExpression(filter))))
					return false;

				var approvedDateValue = entity.Fields[field.Name].CurrentValue as DateTime?;

				// only save date if not already approved
				if (!approvedDateValue.HasValue)
				{
					entity.SetNewFieldValue(field.Name, date);
					return da.SaveEntity(entity, true);
				}
				return true;
			}
		}
	}
}
