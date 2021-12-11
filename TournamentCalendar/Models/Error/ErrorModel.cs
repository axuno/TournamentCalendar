using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TournamentCalendar.Models.Error
{
    public class ErrorModel
    {
        public string OrigPath { get; set; }

        public Exception Exception { get; set; }

        public Status Status { get; } = new Status();
     }

    public class Status
    {
        public string Code { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string GermanText { get; set; }
        public string GermanDescription { get; set; }
    }
}
