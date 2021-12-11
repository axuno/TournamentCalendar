using System;
using System.Threading.Tasks;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;

namespace TournamentCalendar.Models.Shared
{
	public class ApproveModelTournamentCalendar<T> where T : EntityBase2, new()
	{
		private readonly IPredicate _filter;
		private readonly EntityField2 _approveDateField;
		private readonly EntityField2 _deletedOnDateField;

		public ApproveModelTournamentCalendar(IPredicate filter, EntityField2 approveDateField, EntityField2 deletedOnDateField)
		{
			_filter = filter;
			_approveDateField = approveDateField;
			_deletedOnDateField = deletedOnDateField;
			SaveSuccessFul = IsEntityFound = false;
		}
		public bool SaveSuccessFul { get; set; }
		public bool IsEntityFound { get; set; }
		public CalendarEntity PossibleDuplicateFound { get; private set; }
		public Exception Exception { get; set; }
		public T Entity { get; set; }

		public static bool IsGuid(string guid)
		{
            return Guid.TryParse(guid, out var dummy);
        }

	    public async Task<ApproveModelTournamentCalendar<T>> Save()
	    {
	        SaveSuccessFul = false;

	        try
	        {
	            Entity = new T();

	            IsEntityFound = CalendarRepository.FetchEntity(Entity, new PredicateExpression(_filter));

	            if (!IsEntityFound)
	            {
	                return this;
	            }

	            if (Entity is CalendarEntity)
	            {
	                PossibleDuplicateFound =
	                    await CalendarRepository.GetPossibleDuplicate(Entity as CalendarEntity);
	                if (PossibleDuplicateFound != null)
	                    return this;
	            }

	            var deletedOnDateValue = Entity.Fields[_deletedOnDateField.Name].CurrentValue as DateTime?;
	            if (deletedOnDateValue.HasValue)
	            {
	                Entity.SetNewFieldValue(_deletedOnDateField.Name, null);
	            }

	            var approvedDateValue = Entity.Fields[_approveDateField.Name].CurrentValue as DateTime?;
	            // only save date if not already approved
	            if (!approvedDateValue.HasValue)
	            {
	                Entity.SetNewFieldValue(_approveDateField.Name, DateTime.Now);
	            }

	            if (Entity.IsDirty)
	            {
	                SaveSuccessFul = await CalendarRepository.SaveEntity(Entity, true);
	            }
	            else
	            {
	                SaveSuccessFul = true;
	            }
	        }
	        catch (Exception ex)
	        {
	            Exception = ex;
	        }
	        return this;
	    }
	}
}