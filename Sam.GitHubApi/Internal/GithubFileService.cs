using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Sam.GitHubApi.Internal
{
    public class GithubFileService : IGithubFileService
    {
        private readonly HttpClient _httpClient;
        private readonly string _owner;
        private readonly string _repo;

        public GithubFileService(GitHubApiOptions options)
        {
            _httpClient=new HttpClient();

            _owner = options.Owner;
            _repo =options.Repo;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", options.Token);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));
        }

        private string GetUrl(string filePath) =>
            $"https://api.github.com/repos/{_owner}/{_repo}/contents/{filePath}";

        public async Task<string?> GetFileContentAsync(string filePath)
        {
            var response = await _httpClient.GetAsync(GetUrl(filePath));
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var base64Content = doc.RootElement.GetProperty("content").GetString();
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64Content));
        }

        public async Task<bool> CreateOrUpdateFileAsync(string filePath, string content, string commitMessage)
        {
            // Step 1: get SHA if file exists
            string? sha = null;
            var getResponse = await _httpClient.GetAsync(GetUrl(filePath));
            if (getResponse.IsSuccessStatusCode)
            {
                var fileJson = await getResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(fileJson);
                sha = doc.RootElement.GetProperty("sha").GetString();
            }

            // Step 2: prepare payload
            var payload = new
            {
                message = commitMessage,
                content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
                sha = sha
            };

            var json = JsonSerializer.Serialize(payload);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Step 3: send PUT
            var putResponse = await _httpClient.PutAsync(GetUrl(filePath), stringContent);
            return putResponse.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteFileAsync(string filePath, string commitMessage)
        {
            // Must have SHA to delete
            var getResponse = await _httpClient.GetAsync(GetUrl(filePath));
            if (!getResponse.IsSuccessStatusCode) return false;

            var fileJson = await getResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(fileJson);
            string sha = doc.RootElement.GetProperty("sha").GetString();

            var payload = new
            {
                message = commitMessage,
                sha = sha
            };

            var json = JsonSerializer.Serialize(payload);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, GetUrl(filePath))
            {
                Content = stringContent
            };

            var deleteResponse = await _httpClient.SendAsync(deleteRequest);
            return deleteResponse.IsSuccessStatusCode;
        }
    }
}