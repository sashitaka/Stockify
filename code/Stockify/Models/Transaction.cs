namespace Stockify.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public User? User { get; set; }
        public string Type { get; set; } = "";
        public string Ticker { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public decimal Quantity { get; set; }
        public decimal PriceAtTransaction { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; } = "COMPLETED";
    }
}
