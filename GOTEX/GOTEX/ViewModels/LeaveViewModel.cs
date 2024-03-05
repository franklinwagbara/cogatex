using GOTEX.Core.BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GOTEX.ViewModels
{
    public class LeaveFormViewModel
    {
        public LeaveForm Leave { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public bool HasLeaveRequest { get; set; }
    }

    public class LeaveForm
    {
        [DisplayName("Staff Name")]
        public string StaffName { get; set; }
        public string? StaffId { get; set; } 

        [DisplayName("Acting Staff")]
        [Required(ErrorMessage = "Please select a value for Acting Staff")]
        public string ActingStaffId { get; set; }

        [Required(ErrorMessage = "Please select a value for Start DateTime")]
        [DisplayName("Start DateTime")]
        public DateTime Start { get; set; } = DateTime.Now;

        [DisplayName("End DateTime")]
        [Required(ErrorMessage = "Please select a value for End DateTime")]
        public DateTime End { get; set; } = DateTime.Now.AddMonths(1);
        public  bool isDeleted { get; set; } = false;
    }
}
