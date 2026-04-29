
namespace Stockify.Data
{
    
    public class UserSession
    {
        
        public int UserId { get; set; } = -1;
        public string Name { get; set; } = "";
        public bool IsLoggedIn => UserId != -1;

        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public decimal Balance { get; set; } = 0;
    }
}
