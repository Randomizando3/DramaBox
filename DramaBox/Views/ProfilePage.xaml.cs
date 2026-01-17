using DramaBox.Models;

namespace DramaBox.Views;

public partial class ProfilePage : ContentPage
{
    private UserProfile? _profile;

    // IMPORTANTE: construtor vazio para o TabbedPage não quebrar
    public ProfilePage()
    {
        InitializeComponent();
        ApplyMock(); // enquanto não está ligado no Firebase
    }

    // Opcional: quando você tiver o usuário logado, pode usar este
    public ProfilePage(UserProfile profile) : this()
    {
        SetProfile(profile);
    }

    public void SetProfile(UserProfile profile)
    {
        _profile = profile;
        ApplyProfile(profile);
    }

    private void ApplyMock()
    {
        var mock = new UserProfile
        {
            Name = "Dany Lee LW",
            Email = "dany@exemplo.com",
            Plan = "Premium"
        };

        ApplyProfile(mock);

        LikesLbl.Text = "❤️ 12,4k curtidas";
        SavedLbl.Text = "📌 28 salvos";
        PublishedLbl.Text = "🎬 14 ep publicados";
    }

    private void ApplyProfile(UserProfile p)
    {
        NameLbl.Text = string.IsNullOrWhiteSpace(p.Name) ? "Usuário" : p.Name;
        EmailLbl.Text = string.IsNullOrWhiteSpace(p.Email) ? "email@exemplo.com" : p.Email;
        PlanBadgeLbl.Text = string.IsNullOrWhiteSpace(p.Plan) ? "Grátis" : p.Plan;

        // Avatar: iniciais
        var initials = GetInitials(NameLbl.Text);
        AvatarLbl.Text = string.IsNullOrWhiteSpace(initials) ? "U" : initials;
    }

    private static string GetInitials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "U";
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();
        return (parts[0].Substring(0, 1) + parts[^1].Substring(0, 1)).ToUpperInvariant();
    }

    private void OnThemeClicked(object sender, EventArgs e)
    {
        var app = Application.Current;
        if (app == null) return;

        app.UserAppTheme = app.UserAppTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
    }

    private async void OnOpenPlansClicked(object sender, EventArgs e)
    {
        // Não depende de SubscriptionPage existir.
        // Se existir e você quiser ligar depois, é só trocar aqui.
        await DisplayAlert("Planos", "Abrir planos (mock).", "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var ok = await DisplayAlert("Sair", "Deseja encerrar a sessão?", "Sim", "Cancelar");
        if (!ok) return;

        // Aqui você liga no FirebaseAuthService depois (SignOut).
        await Navigation.PopToRootAsync();
    }
}
