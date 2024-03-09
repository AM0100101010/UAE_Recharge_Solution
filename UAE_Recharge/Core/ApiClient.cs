using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UAE_Recharge.Core.Api
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl.TrimEnd('/') + "/";
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(_baseUrl + endpoint);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            else
            {
                throw new Exception($"Failed to retrieve data from API. Status code: {response.StatusCode}");
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl + endpoint, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            else
            {
                throw new Exception($"Failed to post data to API. Status code: {response.StatusCode}");
            }
        }
    }
}
