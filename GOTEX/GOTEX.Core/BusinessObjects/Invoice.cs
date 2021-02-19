using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GOTEX.Core.BusinessObjects
{
    public class Invoice
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public double Amount { get; set; }
        [Required]
        [StringLength(6)]
        public string Status { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name="Payment Code")]
        [JsonProperty("Payment_Code")]
        public string PaymentCode { get; set; }
        [StringLength(50)]
        [JsonProperty("Payment_Type")]
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        [StringLength(25)]
        [Display(Name = "Receipt Number")]
        [JsonProperty("Receipt_No")]
        public string ReceiptNo { get; set; }
        [Display(Name = "Date Opened")]
        [JsonProperty("Date_Added")]
        public DateTime DateAdded { get; set; }
        [Display(Name = "Date Paid")]
        [JsonProperty("Date_Paid")]
        public DateTime DatePaid { get; set; }
        public int PaymentTransactionId { get; set; }
        [ForeignKey("ApplicationId")]
        public Application Application { get; set; }
    }
    public class RemitaSplit
    {
        public string serviceTypeId { get; set; }
        public string totalAmount { get; set; }
        public string hash { get; set; }
        public string payerName { get; set; }
        public string payerEmail { get; set; }
        public string payerPhone { get; set; }
        public string orderId { get; set; }
        public List<RPartner> lineItems { get; set; }
        public string ServiceCharge { get; set; }
        public string AmountDue { get; set; }
        public string ReturnSuccessUrl { get; set; }
        public string ReturnFailureUrl { get; set; }
        public string ReturnBankPaymentUrl { get; set; }
        public List<int> DocumentTypes { get; set; }
        public string CategoryName { get; set; }
        public List<ApplicationItem> ApplicationItems { get; set; }
    }

    public class RPartner
    {
        public string lineItemsId { get; set; }
        public string beneficiaryName { get; set; }
        public string beneficiaryAccount { get; set; }
        public string bankCode { get; set; }
        public string beneficiaryAmount { get; set; }
        public string deductFeeFrom { get; set; }
    }
}