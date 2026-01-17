using DramaBox.Services;

namespace DramaBox.Views;

public partial class RegisterPage : ContentPage
{
    private readonly FirebaseAuthService _auth;

    public RegisterPage()
    {
        InitializeComponent();
        _auth = Application.Current?.Handler?.MauiContext?.Services.GetService<FirebaseAuthService>()
                ?? throw new Exception("DI não disponível.");
    }

    private async void OnRegister(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        try
        {
            var email = (EmailEntry.Text ?? "").Trim();
            var pass = PassEntry.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
                throw new Exception("Informe e-mail e senha.");

            await _auth.SignUpAsync(email, pass);

            await Navigation.PushAsync(new MainTabsPage());
            // remove Register + Login da pilha (se existir)
            var toRemove = Navigation.NavigationStack.Where(p => p is LoginPage || p is RegisterPage).ToList();
            foreach (var p in toRemove) Navigation.RemovePage(p);
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }
}
