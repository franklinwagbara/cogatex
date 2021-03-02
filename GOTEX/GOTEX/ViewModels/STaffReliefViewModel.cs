using System.ComponentModel.DataAnnotations;

namespace GOTEX.ViewModels
{
    public class StaffReliefViewModel
    {
        [Required]
        public string OldStaffEmail { get; set; }
        [Required]
        public string NewStaff { get; set; }
        [Required]
        public int AppNumber { get; set; }
    }
}