using Microsoft.AspNetCore.Identity;

namespace JobPortalProjectMVC.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
