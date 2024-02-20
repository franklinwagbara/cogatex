using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int LeaveId { get; set; }
        public string ApprovingStaffId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateApproved { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(LeaveId))]
        public Leave Leave { get; set; }
        [ForeignKey(nameof(ApprovingStaffId))]
        public ApplicationUser ApprovingStaff { get; set; }
    }
}
