using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace Stockify.Models
{
    public class ApiManagerSingleton
    {

        private const string apiKey = "dp5iFyne5UXzF3KhBbU2dr_ElfWcK1TO";
        private const string baseUrl = "https://api.massive.com/v3/quotes/";
        private readonly HttpClient httpClient;

        private ApiManagerSingleton()
        {
            httpClient = new HttpClient();
        }

        public string ApiKey { get => apiKey;}

        private static readonly Lazy<ApiManagerSingleton> _lazy = new(() => new ApiManagerSingleton());

        public static ApiManagerSingleton getInstance() => _lazy.Value;

        public async Task<string> GetStockValueAsync(string ticker)
        {
            try
            {
                string requestUrl = $"{baseUrl}{ticker}?apiKey={ApiKey}";

                var response = await httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(
                        $"API returned {(int)response.StatusCode}: {response.ReasonPhrase}"
                    );

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }




    }
}
