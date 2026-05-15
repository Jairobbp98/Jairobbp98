using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using ManagementSystem.Mobile.Models;

namespace ManagementSystem.Mobile.Services;

public interface IApiService
{
    void SetAuthToken(string token);
    void ClearAuthToken();
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string firstName, string lastName, string password);
    Task<PagedResult<User>> GetUsersAsync(int page = 1, int limit = 10);
    Task<User> GetUserAsync(string id);
    Task<PagedResult<Company>> GetCompaniesAsync(int page = 1, int limit = 10);
    Task<Company> GetCompanyAsync(string id);
    Task<PagedResult<Document>> GetDocumentsAsync(int page = 1, int limit = 10);
    Task<Document> GetDocumentAsync(string id);
    Task<Document> CreateDocumentAsync(string title, string? content, string type);
    Task<byte[]> ExportDocumentAsPdfAsync(string id);
    Task<string> ExportDocumentAsMarkdownAsync(string id);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string? _accessToken;

    public ApiService(string baseUrl = "http://localhost:3000/api/v1")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public void SetAuthToken(string token)
    {
        _accessToken = token;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthToken()
    {
        _accessToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var body = JsonConvert.SerializeObject(new { email, password });
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("auth/login", content);
        await EnsureSuccessAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<AuthResult>(json)!;
        SetAuthToken(result.AccessToken);
        return result;
    }

    public async Task<AuthResult> RegisterAsync(
        string email, string firstName, string lastName, string password)
    {
        var body = JsonConvert.SerializeObject(new { email, firstName, lastName, password });
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("auth/register", content);
        await EnsureSuccessAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<AuthResult>(json)!;
        SetAuthToken(result.AccessToken);
        return result;
    }

    public async Task<PagedResult<User>> GetUsersAsync(int page = 1, int limit = 10)
    {
        var response = await _httpClient.GetAsync($"users?page={page}&limit={limit}");
        await EnsureSuccessAsync(response);
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PagedResult<User>>(json)!;
    }

    public async Task<User> GetUserAsync(string id)
    {
        var response = await _httpClient.GetAsync($"users/{id}");
        await EnsureSuccessAsync(response);
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<User>(json)!;
    }

    public async Task<PagedResult<Company>> GetCompaniesAsync(int page = 1, int limit = 10)
    {
        var response = await _httpClient.GetAsync($"companies?page={page}&limit={limit}");
        await EnsureSuccessAsync(response);
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PagedResult<Company>>(json)!;
    }

    public async Task<Company> GetCompanyAsync(string id)
    {
        var response = await _httpClient.GetAsync($"companies/{id}");
        await EnsureSuccessAsync(response);
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Company>(json)!;
    }

    public async Task<PagedResult<Document>> GetDocumentsAsync(int page = 1, int limit = 10)
    {
        var response = await _httpClient.GetAsync($"documents?page={page}&limit={limit}");
        await EnsureSuccessAsync(response);
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PagedResult<Document>>(json)!;
    }

    public async Task<Document> GetDocumentAsync(string id)
    {
        var response = await _httpClient.GetAsync($"documents/{id}");
        await EnsureSuccessAsync(response);
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Document>(json)!;
    }

    public async Task<Document> CreateDocumentAsync(string title, string? content, string type = "markdown")
    {
        var body = JsonConvert.SerializeObject(new { title, content, type });
        var httpContent = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("documents", httpContent);
        await EnsureSuccessAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Document>(json)!;
    }

    public async Task<byte[]> ExportDocumentAsPdfAsync(string id)
    {
        var response = await _httpClient.GetAsync($"documents/{id}/export/pdf");
        await EnsureSuccessAsync(response);
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<string> ExportDocumentAsMarkdownAsync(string id)
    {
        var response = await _httpClient.GetAsync($"documents/{id}/export/markdown");
        await EnsureSuccessAsync(response);
        return await response.Content.ReadAsStringAsync();
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            var apiError = JsonConvert.DeserializeObject<ApiError>(error);
            throw new HttpRequestException(
                apiError?.Message ?? $"HTTP Error: {response.StatusCode}",
                null,
                response.StatusCode);
        }
    }
}
