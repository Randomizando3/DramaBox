using DramaBox.Models;
using DramaBox.Services;

namespace DramaBox.Views;

public partial class DiscoverPage : ContentPage
{
    private DramaService? _service;

    public IList<Drama> Items { get; private set; } = new List<Drama>();

    // IMPORTANTE: construtor padrão + classe pública
    public DiscoverPage()
    {
        InitializeComponent();
        BindingContext = this;

        _service = Application.Current?.Handler?.MauiContext?.Services.GetService<DramaService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            _service ??= Application.Current?.Handler?.MauiContext?.Services.GetService<DramaService>();
            if (_service == null) return;

            var list = await _service.GetOfficialSeriesAsync();
            Items = list ?? new List<Drama>();
        }
        catch
        {
            Items = new List<Drama>();
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(Items)));
        }
    }

    private void OnThemeClicked(object sender, EventArgs e)
    {
        var cur = Application.Current?.UserAppTheme ?? AppTheme.Unspecified;
        Application.Current!.UserAppTheme = cur == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
    }

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is Drama drama)
        {
            // abre detalhes e toca primeiro episódio ao clicar em "Assistir"
            await Navigation.PushAsync(new DramaDetailsPage(drama));
        }
    }

    private async void OnDetailsClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is Drama drama)
        {
            await Navigation.PushAsync(new DramaDetailsPage(drama));
        }
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is Drama drama)
        {
            await DisplayAlert("Link", $"Compartilhar: {drama.Title} (mock)", "OK");
        }
    }
}
