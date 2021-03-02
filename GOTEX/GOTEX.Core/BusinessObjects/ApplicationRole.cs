using Microsoft.AspNetCore.Identity;

namespace GOTEX.Core.BusinessObjects
{
    public class ApplicationRole : IdentityRole
    {
        public string DisplayName { get; set; }
    }
}