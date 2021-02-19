using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class WorkFlow
    { 
        public int Id { get; set; }
        public int ApplicationTypeId { get; set; }
        public string Action { get; set; }
        public string TriggeredByRole { get; set; }
        public string TargetRole { get; set; }
        public string Status { get; set; }
        [ForeignKey("ApplicationTypeId")]
        public ApplicationType ApplicationType { get; set; }
    }
}