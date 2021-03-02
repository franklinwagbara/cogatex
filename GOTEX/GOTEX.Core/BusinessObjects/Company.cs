using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class Company
    {
        public int Id { get; set; }
        //public int UserId { get; set; }
        public int ElpsId { get; set; }
        public int RegAddressId { get; set; }
        public string RegisteredAddress { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public string YearIncorporated { get; set; }
        public string RcNumber { get; set; }
        public string TinNumber { get; set; }
        public string DirectorSignature { get; set; }
    }
}