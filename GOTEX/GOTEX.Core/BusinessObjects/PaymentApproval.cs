using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class PaymentApproval
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string Bank { get; set; }
        public string PaymentId { get; set; }
        public string UserId { get; set; }
        public string PaymentUrl { get; set; }
        public decimal Amount { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        [ForeignKey("ApplicationId")]
        public Application Application { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}