using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class Facility
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int TerminalId { get; set; }
        public int ProductId { get; set; }
        public int ElpsId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        [ForeignKey("TerminalId")]
        public Terminal Terminal { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}