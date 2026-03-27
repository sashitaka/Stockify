using Microsoft.AspNetCore.Identity;

namespace Stockify.Models
{
    public class User : IdentityUser
    {
        public decimal Balance { get; set; } = 10000m;
        public string Currency { get; set; } = "USD";
        public string AccountStatus { get; set; } = "ACTIVE";
    }
}
