namespace Stockify.Models
{
    public class WatchlistEntry
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public User? User { get; set; }
        public string Ticker { get; set; } = "";
        public string CompanyName { get; set; } = "";  
    }
}