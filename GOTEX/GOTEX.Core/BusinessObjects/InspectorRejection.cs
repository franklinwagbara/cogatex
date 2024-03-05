using System;

namespace GOTEX.Core.BusinessObjects
{
    public class InspectorRejection
    {
        public int Id { get; set; }
        public string InspectorId { get; set; }
        public int AppId { get; set; }
        public string TargetUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
