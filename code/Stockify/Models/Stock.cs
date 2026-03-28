namespace Stockify.Models
{
    public class Stock
    {
        public string Ticker { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public decimal Price { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public long Volume { get; set; }
        public string Currency { get; set; } = "USD";
    }
}