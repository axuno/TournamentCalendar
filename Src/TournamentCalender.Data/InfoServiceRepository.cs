﻿using System.Linq;
using System.Threading.Tasks;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendarDAL.Linq;

namespace TournamentCalendar.Data;

public class InfoServiceRepository : GenericRepository
{
    public InfoServiceRepository(IDbContext dbContext) : base(dbContext) { }
    
    public virtual bool GetRegistrationByGuid(InfoServiceEntity entity, string guid)
    {
        return GetRegistration(entity, new PredicateExpression(InfoServiceFields.Guid == guid));
    }

    public virtual bool GetRegistrationByEmail(InfoServiceEntity entity, string email)
    {
        var predicate = new FieldCompareValuePredicate(
            InfoServiceFields.Email,
            null,
            ComparisonOperator.Equal,
            email
        )
        {
            // If set to true, the UPPER() function is applied
            CaseSensitiveCollation = true
        };

        var filter = new PredicateExpression(predicate);
        return GetRegistration(entity, filter);
    }

    public virtual bool GetRegistration(InfoServiceEntity entity, PredicateExpression filter)
    {
        using var da = _dbContext.GetNewAdapter();
        var success = da.FetchEntityUsingUniqueConstraint(entity, filter);
        return success;
    }

    public virtual int GetIdForGuid(string guid)
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);

        // If Guid does not exist, Id will be zero:
        return (from t in metaData.InfoService where t.Guid == guid select t.Id).First();
    }

    public virtual async Task <EntityCollection<InfoServiceEntity>> GetActiveSubscribers()
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);
        return await (from subs in metaData.InfoService
            where subs.ConfirmedOn.HasValue select subs).ExecuteAsync<EntityCollection<InfoServiceEntity>>();
    }
}
