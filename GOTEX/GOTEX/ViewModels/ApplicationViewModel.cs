using GOTEX.Core.BusinessObjects;
using System.Collections.Generic;

namespace GOTEX.ViewModels
{
    public class ApplicationViewModel
    {
        public int Id { get; set; }
        public int ApplicationTypeId { get; set; }
        public List<ApplicationType> ApplicationType { get; set; }
        public int Quantity { get; set; }
        public int QuarterId { get; set; }
        public List<Quarter> Quarter { get; set; }
        public int ProductId { get; set; }
        public List<Product> Product { get; set; }
        public int TerminalId { get; set; }
        public List<Terminal> Terminal { get; set; }
        public int StageId { get; set; }
        public decimal Amount { get; set; }
        public string GasStream { get; set; }
        public string LastAssignedUserId { get; set; }
        public string Status { get; set; }
    }
}
