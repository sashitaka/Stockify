using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Stockify.Models
{
    public class ApiManagerSingleton
    {
        private static ApiManagerSingleton? _instance;
        private static readonly object _lockObj = new();

        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        private ApiManagerSingleton(IConfiguration configuration)
        {
            _apiKey = configuration["MassiveApi:ApiKey"] ?? "nqSHuJEa1PCoJNT9cXXjWaJ1WOHK0QqD";
            _baseUrl = configuration["MassiveApi:BaseUrl"] ?? "https://api.massive.com";

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static ApiManagerSingleton getInstance(IConfiguration configuration)
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new ApiManagerSingleton(configuration);
                    }
                }
            }
            return _instance;
        }

        public async Task<string> GetStockValueAsync(string ticker)
        {
            try
            {
                string url = $"{_baseUrl}/v3/quotes/{ticker}?apiKey={_apiKey}";
                System.Diagnostics.Debug.WriteLine($"📡 API Request: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"❌ API Error: {response.StatusCode} - {errorBody}");
                    throw new HttpRequestException($"{(int)response.StatusCode}: {response.ReasonPhrase}");
                }

                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"✅ API Response: {json}");
                return json;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 Exception: {ex.Message}");
                return $"Exception: {ex.Message}";
            }
        }
    }
}
