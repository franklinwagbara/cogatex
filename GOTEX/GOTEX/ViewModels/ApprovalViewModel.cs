namespace GOTEX.ViewModels
{
    public class ApprovalViewModel
    {
        public int APplicationId { get; set; }
        public string Reference { get; set; }
        public string Report { get; set; }
        public string Action { get; set; }
        public bool IsPaymentRelated { get; set; } = false;
    }
}