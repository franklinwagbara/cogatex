using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GOTEX.ViewModels
{
    public class DocumentUpdateViewModel
    {
        [Required]
        public int ApplicationTypeId { get; set; }
        [Required]
        public List<int> DocId { get; set; }
    }
}