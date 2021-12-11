using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendarDAL.Linq;

namespace TournamentCalender.Data
{
    public class SentNewsletterRepository : GenericRepository
    {
        public static async Task<DateTime?> GetLastSendDate()
        {
            using (var da = Connecter.GetNewAdapter())
            {
                var metaData = new LinqMetaData(da);
                return await (from nl in metaData.SentNewsletter select nl.StartedOn).MaxAsync();
            }
        }

        public static async Task<List<SentNewsletterEntity>> GetLastNewsletters()
        {
            using (var da = Connecter.GetNewAdapter())
            {
                var metaData = new LinqMetaData(da);
                return await (from nl in metaData.SentNewsletter orderby nl.StartedOn descending select nl ).Take(4).ToListAsync();
            }
        }
    }
}
