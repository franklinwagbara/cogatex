using System;

namespace GOTEX.Core.BusinessObjects
{
    public class ApplicationType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public Decimal Amount { get; set; }
        public Decimal ServiceCharge { get; set; }
        public Decimal ProcessingFee { get; set; }
        public string IssueType { get; set; }
    }
}