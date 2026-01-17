using System.Text;
using System.Text.Json;
using DramaBox.Config;

namespace DramaBox.Services;

public class FirebaseAuthSession
{
    public string IdToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public string Uid { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; } = DateTime.UtcNow.AddMinutes(-1);

    public bool IsValid => !string.IsNullOrWhiteSpace(IdToken) && DateTime.UtcNow < ExpiresAtUtc;
}

public class FirebaseAuthService
{
    private readonly HttpClient _http = new();

    public FirebaseAuthSession? Current { get; private set; }

    public async Task<FirebaseAuthSession> SignUpAsync(string email, string password)
    {
        var payload = new { email, password, returnSecureToken = true };
        var json = JsonSerializer.Serialize(payload);
        using var res = await _http.PostAsync(FirebaseConfig.SignUpUrl, new StringContent(json, Encoding.UTF8, "application/json"));
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception(ParseFirebaseError(body));

        var doc = JsonDocument.Parse(body).RootElement;

        var expiresIn = doc.GetProperty("expiresIn").GetString() ?? "3600";
        var session = new FirebaseAuthSession
        {
            IdToken = doc.GetProperty("idToken").GetString() ?? "",
            RefreshToken = doc.GetProperty("refreshToken").GetString() ?? "",
            Uid = doc.GetProperty("localId").GetString() ?? "",
            Email = doc.GetProperty("email").GetString() ?? email,
            ExpiresAtUtc = DateTime.UtcNow.AddSeconds(int.TryParse(expiresIn, out var s) ? s : 3600)
        };

        Current = session;
        return session;
    }

    public async Task<FirebaseAuthSession> SignInAsync(string email, string password)
    {
        var payload = new { email, password, returnSecureToken = true };
        var json = JsonSerializer.Serialize(payload);
        using var res = await _http.PostAsync(FirebaseConfig.SignInUrl, new StringContent(json, Encoding.UTF8, "application/json"));
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception(ParseFirebaseError(body));

        var doc = JsonDocument.Parse(body).RootElement;

        var expiresIn = doc.GetProperty("expiresIn").GetString() ?? "3600";
        var session = new FirebaseAuthSession
        {
            IdToken = doc.GetProperty("idToken").GetString() ?? "",
            RefreshToken = doc.GetProperty("refreshToken").GetString() ?? "",
            Uid = doc.GetProperty("localId").GetString() ?? "",
            Email = doc.GetProperty("email").GetString() ?? email,
            ExpiresAtUtc = DateTime.UtcNow.AddSeconds(int.TryParse(expiresIn, out var s) ? s : 3600)
        };

        Current = session;
        return session;
    }

    public void SignOut() => Current = null;

    private static string ParseFirebaseError(string body)
    {
        try
        {
            var root = JsonDocument.Parse(body).RootElement;
            var msg = root.GetProperty("error").GetProperty("message").GetString() ?? "UNKNOWN";
            return msg switch
            {
                "EMAIL_EXISTS" => "Este e-mail já está cadastrado.",
                "INVALID_PASSWORD" => "Senha inválida.",
                "EMAIL_NOT_FOUND" => "E-mail não encontrado.",
                "USER_DISABLED" => "Usuário desabilitado.",
                _ => $"Erro Firebase: {msg}"
            };
        }
        catch { return "Erro inesperado ao autenticar."; }
    }
}
