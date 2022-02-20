using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class Application
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required]
        public int ApplicationTypeId { get; set; }
        public int Year { get; set;}
        public decimal Fee { get; set; }
        public decimal ServiceCharge { get; set; }
        public int StageId { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string LastAssignedUserId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int QuarterId { get; set; }
        [Required]
        public int TerminalId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public int? PaymentEvidenceId { get; set; }
        public int? FacilityId { get; set; }
        public string GasStream { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal ProductAmount { get; set; }
        public string CurrentPermit { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsProcessed { get; set; }
        public bool Submitted { get; set; }
        [ForeignKey("QuarterId")]
        public Quarter Quarter { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("TerminalId")]
        public Terminal Terminal { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [ForeignKey("ApplicationTypeId")]
        public ApplicationType ApplicationType { get; set; }
        [ForeignKey("PaymentEvidenceId")]
        public PaymentEvidence PaymentEvidence { get; set; }
        [ForeignKey("FacilityId")]
        public Facility Facility { get; set; }

        public string DateView => Date.ToLongDateString();
    }
    public class ApplicationItem
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class ApplicationStatus
    {
        public static readonly string NotSubmitted = "Not Submitted";
        public static readonly string PaymentNotSatisfied = "Payment Not Satisfied";
        public static readonly string PaymentPending = "Payment Pending";
        public static readonly string PaymentCompleted = "Payment Completed";
        public static readonly string Processing = "Processing";
        public static readonly string Rejected = "Rejected";
        public static readonly string Completed = "Completed";
    }
}