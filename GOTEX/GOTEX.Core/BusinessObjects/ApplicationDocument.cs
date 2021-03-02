using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class ApplicationDocument
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int DocumentId { get; set; }
        public int DocTypeId { get; set; }
        public int ApplicationTypeDocumentId { get; set; }
        [ForeignKey("ApplicationId")]
        public Application Application { get; set; }
        public string UniqueId { get; set; }
        public bool IsUploaded { get; set; }
    }
}