using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;

namespace Stockify.Business
        private static ApiManagerSingleton? _instance;
        private static readonly object _lockObj = new();

        private readonly string _apiKey;

        private readonly string _apiKey;
        private ApiManagerSingleton(IConfiguration configuration)
        {
            _apiKey = configuration["MassiveApi:ApiKey"] ?? "";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    _instance ??= new ApiManagerSingleton(configuration);
                }
            }
            return _instance;
        }
            }
        public async Task<StockResult?> GetStockAsync(string ticker)
        {

        public async Task<string> GetStockValueAsync(string ticker)
                string url = $"https://api.massive.com/v2/aggs/ticker/{ticker.ToUpper().Trim()}/prev?apiKey={_apiKey}";
                Console.WriteLine($"URL: {url}");
            {
                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"JSON: {json}");
                System.Diagnostics.Debug.WriteLine($"📡 API Request: {url}");
                if (!response.IsSuccessStatusCode) return null;

                var result = JsonSerializer.Deserialize<MassiveAggsResponse>(json);
                return result?.Results?.FirstOrDefault();
                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"✅ API Response: {json}");
                return json;
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
        public double O { get; set; }

        [JsonPropertyName("c")]
        public double C { get; set; }

        [JsonPropertyName("h")]
        public double H { get; set; }

        [JsonPropertyName("l")]
        public double L { get; set; }

        [JsonPropertyName("v")]
        public double V { get; set; }

        [JsonPropertyName("vw")]
        public double Vw { get; set; }

        [JsonPropertyName("t")]
        public long Timestamp { get; set; }
                return $"Exception: {ex.Message}";
            }
        }
    }
}