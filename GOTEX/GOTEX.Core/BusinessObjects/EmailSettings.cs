namespace GOTEX.Core.BusinessObjects
{
    public class EmailSettings
    {
        public string Sender { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string WFile { get; set; }
    }
    public class ConstantDocument
    {
        public string ConstantDocuments { get; set; }
    }
    public class RemitaPartners
    {
        public string AccName_1 { get; set; }
        public string AccBC_1 { get; set; }
        public string Type { get; set; }
        public string Acc_1 { get; set; }
        public string AccDeduct_1 { get; set; }
    }
}