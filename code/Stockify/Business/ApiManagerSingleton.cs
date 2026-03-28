using System.Text.Json.Serialization;

namespace Stockify.Models
{
    public class ApiManagerSingleton
    {
        private static ApiManagerSingleton instance;

        private const string apiKey = "nqSHuJEa1PCoJNT9cXXjWaJ1WOHK0QqD";
        private string baseURL;
        private bool isOnline;
        private JsonConverter jsonConverter;

        private ApiManagerSingleton()
        {
        }

        public string ApiKey { get => apiKey;}
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
