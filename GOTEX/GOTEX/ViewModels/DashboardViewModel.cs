using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.ViewModels
{
    public class DashboardViewModel
    {
        public long All { get; set; }
        public long Approved { get; set; }
        public long AppsTreated { get; set; }
        public long Processing { get; set; }
        public long Declined { get; set; }
        public long MyDesk { get; set; }
        public List<Message> Messages { get; set; }
    }
}