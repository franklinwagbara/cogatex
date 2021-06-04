using System.ComponentModel.DataAnnotations;

namespace GOTEX.ViewModels
{
    public class PasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string CPassword { get; set; }
    }
}