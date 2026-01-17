using DramaBox.Services;

namespace DramaBox.Views;

public partial class LoginPage : ContentPage
{
    private FirebaseAuthService? _auth;

    public LoginPage()
    {
        InitializeComponent();
        // NÃO resolver DI aqui (pode estar nulo no Windows).
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Resolve DI quando a Page já está “viva” e com Handler/MauiContext.
        if (_auth == null)
        {
            var sp = Application.Current?.Handler?.MauiContext?.Services;
            _auth = sp?.GetService<FirebaseAuthService>();
        }
    }

    private async void OnGoRegister(object sender, EventArgs e)
        => await Navigation.PushAsync(new RegisterPage());

    private async void OnLogin(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        try
        {
            // Confirma DI
            if (_auth == null)
            {
                var sp = Application.Current?.Handler?.MauiContext?.Services;
                _auth = sp?.GetService<FirebaseAuthService>();
            }
            if (_auth == null)
                throw new Exception("Serviços ainda não carregaram. Volte e tente novamente.");

            var email = (EmailEntry.Text ?? "").Trim();
            var pass = PassEntry.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
                throw new Exception("Informe e-mail e senha.");

            await _auth.SignInAsync(email, pass);

            // Se você usa Shell, ideal seria Shell.Current.GoToAsync...
            // Mas mantendo seu fluxo:
            await Navigation.PushAsync(new MainTabsPage());
            Navigation.RemovePage(this);
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }
}
