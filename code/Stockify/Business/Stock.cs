using System.Text.Json.Serialization;

namespace Stockify.Models
{
    public class Stock
    {
        // Maps the API's "symbol" or "ticker" to your "Nom"
        [JsonPropertyName("ticker")]
        public string Nom { get; set; } = string.Empty;

        // Maps the API's "price" or "value" to your "Value"
        [JsonPropertyName("price")]
        public decimal Value { get; set; }
    }
}