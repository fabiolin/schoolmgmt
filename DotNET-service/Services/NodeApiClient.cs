using System.Net.Http.Json;
using StudentReportService.Models;

namespace StudentReportService.Services;

public interface INodeApiClient
{
    Task<StudentDto> GetStudentAsync(int studentId, CancellationToken ct = default);
}

/// <summary>
/// Singleton service that authenticates with the Node.js backend and proxies
/// student data requests. Cookies are managed manually because the backend sets
/// them with Secure=true over plain HTTP in development, which CookieContainer
/// would silently discard.
/// </summary>
public sealed class NodeApiClient : INodeApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<NodeApiClient> _logger;

    private readonly SemaphoreSlim _loginLock = new(1, 1);
    private Dictionary<string, string> _cookies = new();
    private DateTime _sessionExpiry = DateTime.MinValue;

    public NodeApiClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<NodeApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<StudentDto> GetStudentAsync(int studentId, CancellationToken ct = default)
    {
        await EnsureSessionAsync(ct);

        var response = await SendAuthenticatedAsync(HttpMethod.Get, $"/api/v1/students/{studentId}", ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Session expired mid-request, re-authenticating.");
            _sessionExpiry = DateTime.MinValue;
            await EnsureSessionAsync(ct);
            response = await SendAuthenticatedAsync(HttpMethod.Get, $"/api/v1/students/{studentId}", ct);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new KeyNotFoundException($"Student with ID {studentId} not found.");

        response.EnsureSuccessStatusCode();

        var student = await response.Content.ReadFromJsonAsync<StudentDto>(ct);
        return student ?? throw new InvalidOperationException("Received null student data from upstream API.");
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private async Task EnsureSessionAsync(CancellationToken ct)
    {
        if (IsSessionValid()) return;

        await _loginLock.WaitAsync(ct);
        try
        {
            if (IsSessionValid()) return; // double-checked after acquiring lock
            await LoginAsync(ct);
        }
        finally
        {
            _loginLock.Release();
        }
    }

    private bool IsSessionValid() =>
        _cookies.ContainsKey("accessToken") && DateTime.UtcNow < _sessionExpiry;

    private async Task LoginAsync(CancellationToken ct)
    {
        var section = _config.GetSection("NodeApi");
        var email    = section["AdminEmail"]    ?? throw new InvalidOperationException("NodeApi:AdminEmail not configured.");
        var password = section["AdminPassword"] ?? throw new InvalidOperationException("NodeApi:AdminPassword not configured.");

        var http    = _httpClientFactory.CreateClient("NodeApi");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/login")
        {
            Content = JsonContent.Create(new { username = email, password })
        };

        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        _cookies        = ParseSetCookieHeaders(response);
        _sessionExpiry  = DateTime.UtcNow.AddMinutes(14); // access token lives 15 min

        _logger.LogInformation("Successfully authenticated with Node.js API.");
    }

    private async Task<HttpResponseMessage> SendAuthenticatedAsync(
        HttpMethod method, string url, CancellationToken ct)
    {
        var http    = _httpClientFactory.CreateClient("NodeApi");
        var request = BuildAuthenticatedRequest(method, url);
        return await http.SendAsync(request, ct);
    }

    private HttpRequestMessage BuildAuthenticatedRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);

        if (_cookies.Count > 0)
        {
            var cookieHeader = string.Join("; ", _cookies.Select(kv => $"{kv.Key}={kv.Value}"));
            request.Headers.Add("Cookie", cookieHeader);
        }

        if (_cookies.TryGetValue("csrfToken", out var csrfToken))
            request.Headers.Add("x-csrf-token", csrfToken);

        return request;
    }

    /// <summary>
    /// Extracts name=value pairs from Set-Cookie response headers without relying
    /// on CookieContainer so that the Secure flag does not filter out cookies
    /// received over plain HTTP in development.
    /// </summary>
    private static Dictionary<string, string> ParseSetCookieHeaders(HttpResponseMessage response)
    {
        var cookies = new Dictionary<string, string>();

        if (!response.Headers.TryGetValues("Set-Cookie", out var setCookieValues))
            return cookies;

        foreach (var header in setCookieValues)
        {
            // Format: "name=value; Path=/; HttpOnly; SameSite=Lax; Secure"
            var mainPart  = header.Split(';')[0].Trim();
            var eqIndex   = mainPart.IndexOf('=');
            if (eqIndex < 1) continue;

            var name  = mainPart[..eqIndex].Trim();
            var value = mainPart[(eqIndex + 1)..].Trim();

            if (!string.IsNullOrEmpty(name))
                cookies[name] = value;
        }

        return cookies;
    }
}
