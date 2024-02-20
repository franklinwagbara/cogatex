using System;
using System.ComponentModel.DataAnnotations;

namespace GOTEX.ViewModels
{
    public class DateRangeFilterViewModel
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
    }
}
