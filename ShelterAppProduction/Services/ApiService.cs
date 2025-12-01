using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShelterAppProduction.Services
{
    public class ApiService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string BASE_URL = "http://localhost:8000/api/v1";
        private static string authToken = null;

        static ApiService()
        {
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public static void SetAuthToken(string token)
        {
            authToken = token;
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public static string GetAuthToken()
        {
            return authToken;
        }

        public static async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await httpClient.GetAsync($"{BASE_URL}/{endpoint}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка сети: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex.Message}");
            }
        }

        public static async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{BASE_URL}/{endpoint}", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    throw new Exception(errorResponse?.Detail ?? $"HTTP {(int)response.StatusCode}");
                }

                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка сети: {ex.Message}");
            }
        }

        public static async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{BASE_URL}/{endpoint}", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    throw new Exception(errorResponse?.Detail ?? $"HTTP {(int)response.StatusCode}");
                }

                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка сети: {ex.Message}");
            }
        }

        public static async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"{BASE_URL}/{endpoint}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка сети: {ex.Message}");
            }
        }

        private class ErrorResponse
        {
            public string Detail { get; set; }
        }
    }
}
