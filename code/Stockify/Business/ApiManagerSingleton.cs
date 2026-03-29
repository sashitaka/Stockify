using System.Net.Http.Json;
using Stockify.Models;

namespace Stockify.Models
{
    public class ApiManagerSingleton
    {
        private readonly HttpClient _httpClient;

        // It's best practice to keep the base URL and API Key separate
        private const string BaseUrl = "https://api.massive.com/v3/reference/tickers";
        private const string ApiKey = "dp5iFyne5UXzF3KhBbU2dr_ElfWcK1TO";

        public ApiManagerSingleton(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Stock?> GetStockDetailsAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker)) return null;

            try
            {
                // Use the ticker parameter and ensure it's uppercase for the API
                var url = $"{BaseUrl}/{ticker.ToUpper()}?apiKey={ApiKey}";

                var response = await _httpClient.GetFromJsonAsync<MassiveResponse>(url);

                return response?.Results;
            }
            catch (HttpRequestException httpEx)
            {
                // This will catch 404s (stock not found) or connection issues
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return null;
            }
        }
    }
}