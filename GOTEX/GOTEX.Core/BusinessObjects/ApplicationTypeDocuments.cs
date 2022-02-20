using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GOTEX.Core.BusinessObjects
{
    public class ApplicationTypeDocuments
    {
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public int ApplicationTypeId { get; set; }
        public string DocType { get; set; }
        [ForeignKey("ApplicationTypeId")]
        public ApplicationType ApplicationType { get; set; }
    }
}