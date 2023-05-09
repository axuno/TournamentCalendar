using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SD.LLBLGen.Pro.LinqSupportClasses;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.Linq;

namespace TournamentCalender.Data;

public class SentNewsletterRepository : GenericRepository
{
    public SentNewsletterRepository(IDbContext dbContext) : base(dbContext) { }

    public virtual async Task<DateTime?> GetLastSendDate()
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);
        return await (from nl in metaData.SentNewsletter select nl.StartedOn).MaxAsync();
    }

    public virtual async Task<List<SentNewsletterEntity>> GetLastNewsletters(CancellationToken cancellationToken)
    {
        using var da = _dbContext.GetNewAdapter();
        var metaData = new LinqMetaData(da);
        return await (from nl in metaData.SentNewsletter orderby nl.StartedOn descending select nl ).Take(4).ToListAsync(cancellationToken);
    }
}
