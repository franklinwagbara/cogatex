using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GOTEX.ViewModels
{
    public class DocumentUpdateViewModel
    {
        [Required]
        public int ApplicationTypeId { get; set; }
        public List<string> DocInfo { get; set; }
    }
}