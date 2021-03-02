namespace GOTEX.ViewModels
{
    public class ConfirmPaymentViewModel
    {
        public string refno { get; set; }
        public string feedback { get; set; }
        public double amount { get; set; }
        public string submitBtn { get; set; }
        public string bank { get; set; }
        public string bankPayId { get; set; }
    }
}