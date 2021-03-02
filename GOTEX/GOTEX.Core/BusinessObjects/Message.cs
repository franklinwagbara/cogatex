using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class Message
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}