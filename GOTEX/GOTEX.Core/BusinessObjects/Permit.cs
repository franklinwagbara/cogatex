using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class Permit
    {
        public int Id { get; set; }
        public string PermitNumber { get; set; }
        public string UserId { get; set; }
        public int ApplicationId { get; set; }
        public string LicenseName { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Printed { get; set; }
        public string IsRenewed { get; set; }
        public int ElpsId { get; set; }
        [ForeignKey("ApplicationId")]
        public Application Application { get; set; }
    }
}