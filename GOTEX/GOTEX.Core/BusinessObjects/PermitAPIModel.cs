using System;

namespace GOTEX.Core.BusinessObjects
{
    public class PermitAPIModel
    {
        public int Id { get; set; }
        public string Permit_No { get; set; }

        public string OrderId { get; set; }

        public int Company_Id { get; set; }

        public DateTime Date_Issued { get; set; }

        public DateTime Date_Expire { get; set; }
        public string CategoryName { get; set; }
        public string Is_Renewed { get; set; }
        public int LicenseId { get; set; }
    }
}