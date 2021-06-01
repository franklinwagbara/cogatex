using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GOTEX.Core.BusinessObjects
{
    public class ApplicationUser : IdentityUser
    {
        public int? CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool ProfileComplete { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }

    }
}
