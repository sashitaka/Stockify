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

        private ApiManagerSingleton(IConfiguration configuration)
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


        public async Task<StockResult?> GetStockAsync(string input)
        {
            string? ticker = input.Trim().ToUpper();

            // If the input contains spaces or looks like a company name (not a typical ticker),
            // search for it first
            if (input.Contains(" ") || input.Length >= 5)
            {
                var searchedTicker = await GetTickerBySearchAsync(input);
                if (!string.IsNullOrEmpty(searchedTicker))
                {
                    ticker = searchedTicker;
                }
                else if (input.Length >= 5)
                {
                    // If search failed and input is >= 5 chars, it's probably not a valid ticker
                    return null;
                }
                // If search failed but input is < 5 chars, try it as a ticker anyway
            }

            if (string.IsNullOrEmpty(ticker)) return null;

            try
            {
                string priceUrl = $"https://api.massive.com/v2/aggs/ticker/{ticker}/prev?adjusted=true&apiKey={_apiKey}";
                var response = await _httpClient.GetAsync(priceUrl);
                var json = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"Stock Data URL: {priceUrl}");
                Console.WriteLine($"Stock Data JSON: {json}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API returned error: {response.StatusCode}");
                    return null;
                }

                var priceResponse = JsonSerializer.Deserialize<MassiveAggsResponse>(json);
                var result = priceResponse?.Results?.FirstOrDefault();
                
                if (result != null)
                {
                    result.Ticker = ticker;
                }
                else
                {
                    Console.WriteLine("No results found in API response");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Price Fetch Error: {ex.Message}");
                return null;
            }
        }


        public async Task<string?> GetTickerBySearchAsync(string searchQuery)
        {
            try
            {
                // We limit to 1 result to get the most relevant match
                string url = $"https://api.massive.com/v3/reference/tickers?search={Uri.EscapeDataString(searchQuery)}&active=true&limit=1&apiKey={_apiKey}";

                var response = await _httpClient.GetFromJsonAsync<MassiveTickerResponse>(url);
                return response?.Results?.FirstOrDefault()?.Ticker;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Returns the current closing price of a stock based on a company name search.
        /// </summary>
        /// <param name="companyName">The name of the company (e.g., "Microsoft")</param>
        /// <returns>The price as a double, or 0 if not found.</returns>
        public async Task<double> GetPriceByNameAsync(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName)) return 0;

            try
            {
                string searchUrl = $"https://api.massive.com/v3/reference/tickers?search={Uri.EscapeDataString(companyName)}&active=true&limit=1&apiKey={_apiKey}";

                var searchResponse = await _httpClient.GetFromJsonAsync<MassiveTickerResponse>(searchUrl);
                string? ticker = searchResponse?.Results?.FirstOrDefault()?.Ticker;

                if (string.IsNullOrEmpty(ticker))
                {
                    Console.WriteLine($"Could not find a ticker for: {companyName}");
                    return 0;
                }

                string priceUrl = $"https://api.massive.com/v2/aggs/ticker/{ticker.ToUpper()}/prev?adjusted=true&apiKey={_apiKey}";

                // Log the raw response first
                var httpResponse = await _httpClient.GetAsync(priceUrl);
                var jsonContent = await httpResponse.Content.ReadAsStringAsync();
                
                Console.WriteLine($"Price API Response for {ticker}: {jsonContent}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {httpResponse.StatusCode}");
                    return 0;
                }

                var priceResponse = JsonSerializer.Deserialize<MassiveAggsResponse>(jsonContent);
                var result = priceResponse?.Results?.FirstOrDefault();

                return result?.Close ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching price for {companyName}: {ex.Message}\n{ex.StackTrace}");
                return 0;
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
        [JsonPropertyName("ticker")] public string? Ticker { get; set; }

        [JsonPropertyName("o")] public double Open { get; set; }

        [JsonPropertyName("c")] public double Close { get; set; }

        [JsonPropertyName("h")] public double High { get; set; }

        [JsonPropertyName("l")] public double Low { get; set; }

        [JsonPropertyName("v")] public double Volume { get; set; }

        [JsonPropertyName("vw")] public double VWAP { get; set; }

        [JsonPropertyName("t")] public long Timestamp { get; set; }

        [JsonPropertyName("n")] public long N { get; set; }
    }
        // This matches the top-level JSON object from the /v3/reference/tickers endpoint
        public class MassiveTickerResponse
        {
            [JsonPropertyName("results")]
            public List<TickerSearchResult>? Results { get; set; }

            [JsonPropertyName("status")]
            public string? Status { get; set; }
        }

        // This matches each individual result in the list
        public class TickerSearchResult
        {
            [JsonPropertyName("ticker")]
            public string Ticker { get; set; } = string.Empty;

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("market")]
            public string? Market { get; set; }

            [JsonPropertyName("locale")]
            public string? Locale { get; set; }
        }
}
