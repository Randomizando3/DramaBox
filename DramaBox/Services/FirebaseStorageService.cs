// Services/FirebaseStorageService.cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DramaBox.Config;

namespace DramaBox.Services;

public class FirebaseStorageUploadResult
{
    public bool Ok { get; set; }
    public string Path { get; set; } = "";
    public string Name { get; set; } = "";
    public string Bucket { get; set; } = "";
    public string DownloadToken { get; set; } = "";
    public string MediaUrl { get; set; } = "";
    public string RawJson { get; set; } = "";
}

public class FirebaseStorageService
{
    private readonly HttpClient _http;

    public FirebaseStorageService()
    {
        _http = new HttpClient();
        _http.Timeout = TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// Upload simples via endpoint "uploadType=media" (sem metadata complexa).
    /// pathEx: "covers/dr1.jpg" ou "videos/dr1/ep1.mp4"
    /// contentTypeEx: "image/jpeg", "video/mp4"
    /// idToken: token do usuário logado (necessário se regras exigirem auth).
    /// </summary>
    public async Task<FirebaseStorageUploadResult> UploadBytesAsync(
        byte[] bytes,
        string path,
        string contentType,
        string? idToken = null)
    {
        path = NormalizePath(path);

        var url =
            $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.StorageBucket}/o" +
            $"?uploadType=media&name={Uri.EscapeDataString(path)}";

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new ByteArrayContent(bytes);
        req.Content.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);

        if (!string.IsNullOrWhiteSpace(idToken))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

        using var res = await _http.SendAsync(req);
        var json = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception($"Storage upload falhou ({(int)res.StatusCode}): {Truncate(json)}");

        return ParseUpload(json, path);
    }

    /// <summary>
    /// Deleta um arquivo do Storage.
    /// pathEx: "covers/dr1.jpg"
    /// </summary>
    public async Task DeleteAsync(string path, string? idToken = null)
    {
        path = NormalizePath(path);

        var url = $"{FirebaseConfig.StorageBase}/{Uri.EscapeDataString(path)}";

        using var req = new HttpRequestMessage(HttpMethod.Delete, url);

        if (!string.IsNullOrWhiteSpace(idToken))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

        using var res = await _http.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception($"Storage delete falhou ({(int)res.StatusCode}): {Truncate(body)}");
    }

    /// <summary>
    /// Monta URL de mídia do Firebase Storage.
    /// Se tiver token, inclui. Se não, tenta sem token (funciona se regras permitirem read público).
    /// </summary>
    public string BuildMediaUrl(string path, string? downloadToken = null)
    {
        path = NormalizePath(path);

        var url = $"{FirebaseConfig.StorageBase}/{Uri.EscapeDataString(path)}?alt=media";
        if (!string.IsNullOrWhiteSpace(downloadToken))
            url += $"&token={Uri.EscapeDataString(downloadToken)}";
        return url;
    }

    private static FirebaseStorageUploadResult ParseUpload(string json, string path)
    {
        var result = new FirebaseStorageUploadResult
        {
            Ok = true,
            Path = path,
            RawJson = json
        };

        try
        {
            var root = JsonDocument.Parse(json).RootElement;

            result.Name = root.TryGetProperty("name", out var n) ? (n.GetString() ?? path) : path;
            result.Bucket = root.TryGetProperty("bucket", out var b) ? (b.GetString() ?? FirebaseConfig.StorageBucket) : FirebaseConfig.StorageBucket;

            // Firebase costuma retornar "downloadTokens" dentro de metadata como string com tokens separados por vírgula
            // Ex: "downloadTokens": "uuid-...."
            if (root.TryGetProperty("downloadTokens", out var dt))
            {
                var tokenRaw = dt.GetString() ?? "";
                result.DownloadToken = tokenRaw.Split(',').Select(x => x.Trim()).FirstOrDefault() ?? "";
            }

            result.MediaUrl = new FirebaseStorageService().BuildMediaUrl(result.Name, result.DownloadToken);
        }
        catch
        {
            // Mesmo que falhe parse, ainda retorna ok=true com raw json
            result.MediaUrl = new FirebaseStorageService().BuildMediaUrl(path, null);
        }

        return result;
    }

    private static string NormalizePath(string path)
        => (path ?? "").Trim().TrimStart('/');

    private static string Truncate(string? s, int max = 500)
    {
        s ??= "";
        s = s.Replace("\r", " ").Replace("\n", " ").Trim();
        return s.Length <= max ? s : s.Substring(0, max) + "...";
    }
}
