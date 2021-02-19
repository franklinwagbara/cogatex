namespace GOTEX.Core.BusinessObjects
{
    public class PaymentEvidence
    {
        public int Id { get; set; }
        public int ReferenceType { get; set; }
        public string ReferenceCode { get; set; }
        public string HashCode { get; set; }
        public int ApplicationQuantity { get; set; }
        public decimal Amount { get; set; }
        public int UsageCount { get; set; }
        public string Description { get; set; }
    }
}