﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalender.Data;

namespace TournamentCalendar.Data;

public class GenericRepository
{
    protected static readonly ILogger Logger = AppLogging.CreateLogger<GenericRepository>();
    protected readonly IDbContext _dbContext;

    public GenericRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<bool> Save<T>(T registration, bool refetchAfterSave, CancellationToken cancellationToken) where T: IEntity2
    {
        using var da = _dbContext.GetNewAdapter();
        var success = await da.SaveEntityAsync(registration, refetchAfterSave, cancellationToken);
        return success;
    }

    public virtual bool UpdateDateIfNotSet<T>(T entity, IPredicate filter, EntityField2 field, DateTime date , out bool found) where T: IEntity2, new()
    {
        using var da = _dbContext.GetNewAdapter();
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
