using System.ComponentModel.DataAnnotations;

namespace GOTEX.ViewModels
{
    public class PaymentEvidenceViewModel
    {
        [Required]
        public int ReferenceType { get; set; }
        [Required]
        public string ReferenceCode { get; set; }
        public int ApplicationQuantity { get; set; }
        public decimal Amount { get; set; }
        [Required]
        public string Description { get; set; }
    }
}