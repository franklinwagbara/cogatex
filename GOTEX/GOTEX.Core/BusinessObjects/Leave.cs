using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class Leave
    {
        public int Id { get; set; }
        public string StaffId { get; set; }
        public string ActingStaffId { get; set; }
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        [DisplayName("Staff")]
        [ForeignKey(nameof(StaffId))]
        public ApplicationUser Staff { get; set; }

        [DisplayName("Acting Staff")]
        [ForeignKey(nameof(ActingStaffId))]
        public ApplicationUser ActingStaff { get; set; }
    }
}
