using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
namespace Stockify.Business
{
    public class ApiManagerSingleton
    {
        private static ApiManagerSingleton? _instance;
        private static readonly object _lockObj = new();

        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public ApiManagerSingleton(IConfiguration configuration)
        {
            _apiKey = configuration["MassiveApi:ApiKey"] ?? "";
            _httpClient = new HttpClient();
        }
        public static ApiManagerSingleton getInstance(IConfiguration configuration)
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    _instance ??= new ApiManagerSingleton(configuration);
                }
            }
            return _instance;
        }
        public async Task<StockResult?> GetStockAsync(string ticker)
        {
            try
            {
                string url = $"https://api.massive.com/v2/aggs/ticker/{ticker.ToUpper().Trim()}/prev?apiKey={_apiKey}";
                Console.WriteLine($"URL: {url}");

                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"JSON: {json}");

                if (!response.IsSuccessStatusCode) return null;

                var result = JsonSerializer.Deserialize<MassiveAggsResponse>(json);
                return result?.Results?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
    }
    public class MassiveAggsResponse
    {
        [JsonPropertyName("ticker")]
        public string? Ticker { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("results")]
        public List<StockResult>? Results { get; set; }
    }
    public class StockResult
    {
        [JsonPropertyName("T")]
        public string? Ticker { get; set; }

        [JsonPropertyName("o")]
        public double Open { get; set; }

        [JsonPropertyName("c")]
        public double Close { get; set; }

        [JsonPropertyName("h")]
        public double High { get; set; }

        [JsonPropertyName("l")]
        public double Low { get; set; }

        [JsonPropertyName("v")]
        public double Volume { get; set; }

        [JsonPropertyName("vw")]
        public double VWAP { get; set; }

        [JsonPropertyName("t")]
        public long Timestamp { get; set; }

        [JsonPropertyName("n")]
        public long N { get; set; }
    }
}
