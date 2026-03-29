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
        private static ApiManagerSingleton instance;

        private const string apiKey = "dp5iFyne5UXzF3KhBbU2dr_ElfWcK1TO";
        private const string baseUrl = "https://api.massive.com/v3/quotes/";
        private readonly HttpClient httpClient;

        private ApiManagerSingleton()
        {
            httpClient = new HttpClient();
        }

        public string ApiKey { get => apiKey;}

        public static ApiManagerSingleton getInstance()
        {
            if (instance == null)
            {
                instance = new ApiManagerSingleton();
            }
                return instance;
            
        }

        public async Task<string> GetStockValueAsync(string ticker)
        {
            try
            {
                string requestUrl = $"{baseUrl}{ticker}?apiKey={ApiKey}";

                var response = await httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                return $"Error: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }




    }
}
