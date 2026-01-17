namespace DramaBox.Views;

public partial class CreatorPage : ContentPage
{
    public CreatorPage()
    {
        InitializeComponent();
    }

    private async void OnThemeClicked(object sender, EventArgs e)
    {
        var isDark = Application.Current?.UserAppTheme == AppTheme.Dark;
        Application.Current!.UserAppTheme = isDark ? AppTheme.Light : AppTheme.Dark;
        await Task.CompletedTask;
    }

    private async void OnNewSeriesTapped(object sender, EventArgs e)
    {
        await DisplayAlert("DramaBox", "Nova série (em breve: abrir fluxo de criação/upload).", "OK");
    }
}
