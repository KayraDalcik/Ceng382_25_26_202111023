using Microsoft.AspNetCore.Identity;

namespace Week5.Models
{
    public class ApplicationUser : IdentityUser
    {
        // İsteğe bağlı alanlar (zorunlu değil)
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

    }
}
