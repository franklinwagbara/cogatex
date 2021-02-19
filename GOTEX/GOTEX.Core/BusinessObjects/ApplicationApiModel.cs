using System;

namespace GOTEX.Core.BusinessObjects
{
    public class ApplicationApiModel
    {
        public int id { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string CategoryName { get; set; }
        public int CompanyId { get; set; }
        public int LicenseId { get; set; }
        public DateTime Date { get; set; }
        public string licenseName { get; set; }
    }
}