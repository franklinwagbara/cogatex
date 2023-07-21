using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GOTEX.Core.BusinessObjects
{
    public class Survey
    {
        public int Id { get; set; }
        [Required]
        public int PermitId { get; set; }
        [Display(Name = "How would you rate the overall effectiveness of the process?")]
        [Required]
        public int Effectiveness { get; set; }
        [Required]
        [Display(Name = "Issuance Time")]
        public int IssuanceTime { get; set; }
        [Required]
        [Display(Name = "Payment Process")]
        public int PaymentProcess { get; set; }
        [Required]
        [Display(Name = "Document Upload")]
        public int DocumentUpload { get; set; }
        [Required]
        [Display(Name = "Knowledge Gain")]
        public int KnowledgeGain { get; set; }
        [Required]
        [Display(Name = "User Friendly")]
        public int UserFriendly { get; set; }
        [Required]
        [Display(Name = "Has this permit process added value to your organization?")]
        public int PermitProcess { get; set; }
        [Required]
        [Display(Name = "Please share any other information on how to serve you better")]
        public string Comment { get; set; }
    }
}
