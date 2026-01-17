using System.Net;
using System.Text.Json;
using DramaBox.Config;

namespace DramaBox.Services;

public class FirebaseDatabaseService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json;

    public FirebaseDatabaseService(HttpClient? httpClient = null)
    {
        _http = httpClient ?? new HttpClient();

        _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private string BuildUrl(string path)
    {
        path = (path ?? "").Trim().Trim('/');

        var baseUrl = (FirebaseConfig.RealtimeBaseUrl ?? "").Trim().TrimEnd('/');
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("FirebaseConfig.RealtimeBaseUrl não configurado.");

        var url = $"{baseUrl}/{path}.json";

        var token = (FirebaseConfig.RealtimeAuthToken ?? "").Trim();
        if (!string.IsNullOrEmpty(token))
        {
            url += (url.Contains("?") ? "&" : "?") + "auth=" + Uri.EscapeDataString(token);
        }

        return url;
    }

    public async Task<T?> GetAsync<T>(string path, CancellationToken ct = default)
    {
        var url = BuildUrl(path);

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var res = await _http.SendAsync(req, ct);

        if (res.StatusCode == HttpStatusCode.NotFound)
            return default;

        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null")
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json, _json);
        }
        catch
        {
            return default;
        }
    }
}
