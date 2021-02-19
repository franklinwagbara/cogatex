using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GOTEX.Core.BusinessObjects
{
    public class RemitaPayment
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        [Required]
        [StringLength(50)]
        [JsonProperty("Transaction_Date")]
        public string TransactionDate { get; set; }
        [StringLength(50)]
        [JsonProperty("Reference_Number")]
        public string ReferenceNumber { get; set; }
        [StringLength(50)]
        [JsonProperty("Online_Reference")]
        public string OnlineReference { get; set; }
        [StringLength(100)]
        [JsonProperty("Payment_Reference")]
        public string PaymentReference { get; set; }
        [StringLength(10)]
        [JsonProperty("Approved_Amount")]
        public string ApprovedAmount { get; set; }
        [StringLength(100)]
        [JsonProperty("Response_Description")]
        public string ResponseDescription { get; set; }
        [StringLength(50)]
        [JsonProperty("Response_Code")]
        public string ResponseCode { get; set; }
        [StringLength(10)]
        [JsonProperty("Transaction_Amount")]
        public string TransactionAmount { get; set; }
        [StringLength(5)]
        [JsonProperty("Transaction_Currency")]
        public string TransactionCurrency { get; set; }
        [StringLength(200)]
        [JsonProperty("Customer_Name")]
        public string CustomerName { get; set; }
        [JsonProperty("Customer_Id")]
        public string UserId { get; set; }
        [Required]
        [StringLength(20)]
        [JsonProperty("Order_Id")]
        public string OrderId { get; set; }
        [StringLength(50)]
        [JsonProperty("Payment_Log_Id")]
        public string PaymentLogId { get; set; }
        [Column(TypeName = "datetime2")]
        [JsonProperty("Query_Date")]
        public DateTime QueryDate { get; set; }
        [JsonProperty("Webpay_Reference")]
        public string WebpayReference { get; set; }
        public string RRR { get; set; }
        public string PaymentSource { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
    public class PrePaymentResponse
    {
        public string StatusMessage { get; set; }
        public string AppId { get; set; }
        public string Status { get; set; }
        public string RRR { get; set; }
        public string Transactiontime { get; set; }
        public string TransactionId { get; set; }
    }
}