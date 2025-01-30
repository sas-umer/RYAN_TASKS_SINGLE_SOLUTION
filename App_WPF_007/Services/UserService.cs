using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using BL.Models;

namespace App_WPF_007.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(App.Configuration["ConnectionStrings:ApiBaseUrl"] + "/") };
        }

        public async Task<List<User>> GetUsers()
        {
            var requestUri = "api/Users";
            var response = await _httpClient.GetAsync(requestUri);
            LogRequest(response, requestUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request to {requestUri} failed with status {response.StatusCode}: {errorContent}");
            }
        }

        public async Task<User> GetUser(int id)
        {
            var requestUri = $"api/Users/{id}";
            var response = await _httpClient.GetAsync(requestUri);
            LogRequest(response, requestUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request to {requestUri} failed with status {response.StatusCode}: {errorContent}");
            }
        }

        public async Task<HttpResponseMessage> CreateUser(User user)
        {
            var requestUri = "api/Users";
            var response = await _httpClient.PostAsJsonAsync(requestUri, user);
            LogRequest(response, requestUri);

            return response;
        }

        public async Task<HttpResponseMessage> UpdateUser(int id, User user)
        {
            var requestUri = $"api/Users/{id}";
            var response = await _httpClient.PutAsJsonAsync(requestUri, user);
            LogRequest(response, requestUri);

            return response;
        }

        public async Task<HttpResponseMessage> DeleteUser(int id)
        {
            var requestUri = $"api/Users/{id}";
            var response = await _httpClient.DeleteAsync(requestUri);
            LogRequest(response, requestUri);

            return response;
        }

        private void LogRequest(HttpResponseMessage response, string requestUri)
        {
            var logMessage = $"Request URI: {_httpClient.BaseAddress}{requestUri}\nResponse Status: {response.StatusCode}";
            MessageBox.Show(logMessage);
        }
    }
}
