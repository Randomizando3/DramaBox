namespace DramaBox.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(700);

        // Se quiser auto-login depois, verificar sessão aqui.
        await Navigation.PushAsync(new LoginPage());
        Navigation.RemovePage(this);
    }
}
