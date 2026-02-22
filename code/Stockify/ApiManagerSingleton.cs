using System.Text.Json.Serialization;

namespace Stockify
{
    public class ApiManagerSingleton
    {
        private static ApiManagerSingleton instance;

        private string apiKey;
        private string baseURL;
        private bool isOnline;
        private JsonConverter jsonConverter;

        private ApiManagerSingleton()
        {
        }

        public string ApiKey { get => apiKey; set => apiKey = value; }
        public string BaseURL { get => baseURL; set => baseURL = value; }
        public bool IsOnline { get => isOnline; set => isOnline = value; }
        public JsonConverter JsonConverter { get => jsonConverter; set => jsonConverter = value; }

        public static ApiManagerSingleton getInstance()
        {
            if (instance == null)
            {
                instance = new ApiManagerSingleton();
            }
                return instance;
            
        }
    }
}
