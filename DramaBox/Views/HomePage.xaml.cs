using DramaBox.Models;
using DramaBox.Services;
using DramaBox.ViewModels;

namespace DramaBox.Views;

public partial class HomePage : ContentPage
{
    private HomeViewModel? _vm;
    private DramaService? _dramaService;

    // Construtor padrão obrigatório (XAML do TabbedPage precisa disso)
    public HomePage()
    {
        InitializeComponent();
        ResolveServices();
    }

    private void ResolveServices()
    {
        try
        {
            var sp = Application.Current?.Handler?.MauiContext?.Services;
            if (sp == null) return;

            _vm = sp.GetService<HomeViewModel>();
            if (_vm != null)
                BindingContext = _vm;

            _dramaService = sp.GetService<DramaService>();
        }
        catch
        {
            // tenta novamente no OnAppearing
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_vm == null || _dramaService == null)
            ResolveServices();

        if (_vm != null)
            await _vm.LoadAsync();
    }

    // -------------------- HANDLERS DO XAML --------------------

    private void OnThemeClicked(object sender, EventArgs e)
    {
        var current = Application.Current?.UserAppTheme ?? AppTheme.Unspecified;
        Application.Current!.UserAppTheme = current == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
    }

    private async void OnSearchCompleted(object sender, EventArgs e)
    {
        var q = (SearchEntry?.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(q)) return;

        await DisplayAlert("Pesquisar", $"Busca: {q}", "OK");
    }

    private async void OnRewardsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Recompensas", "Mock por enquanto.", "OK");
    }

    private async void OnVipClicked(object sender, EventArgs e)
    {
        await DisplayAlert("VIP", "Mock por enquanto.", "OK");
    }

    private async void OnHeroPlayClicked(object sender, EventArgs e)
    {
        try
        {
            _dramaService ??= Application.Current?.Handler?.MauiContext?.Services.GetService<DramaService>();
            if (_dramaService == null)
                throw new Exception("DramaService não disponível.");

            if (HeroCarousel?.CurrentItem is not DramaService.HeroItemVm hero)
                return;

            var dramaId = (hero.DramaId ?? "").Trim();
            if (string.IsNullOrWhiteSpace(dramaId))
                return;

            var seasonId = string.IsNullOrWhiteSpace(hero.SeasonId) ? "s1" : hero.SeasonId.Trim();
            var episodeId = (hero.EpisodeId ?? "").Trim();

            var drama = await _dramaService.GetDramaByIdAsync(dramaId);
            if (drama == null)
                return;

            var eps = await _dramaService.GetEpisodesAsync(dramaId, seasonId);
            if (eps == null || eps.Count == 0)
                return;

            Episode? ep = null;

            if (!string.IsNullOrWhiteSpace(episodeId))
                ep = eps.FirstOrDefault(x => string.Equals(x.Id, episodeId, StringComparison.OrdinalIgnoreCase));

            ep ??= eps.OrderBy(x => x.Order).ThenBy(x => x.Number).FirstOrDefault();
            if (ep == null)
                return;

            await Navigation.PushAsync(new PlayerPage(drama, ep));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void OnChipClicked(object sender, EventArgs e)
    {
        if (sender is Button b)
            await DisplayAlert("Filtro", $"Filtro: {b.Text}", "OK");
    }

    private async void OnSeeAllTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Ver tudo", "Mock por enquanto.", "OK");
    }

    private async void OnNewCardTapped(object sender, EventArgs e)
    {
        await OpenDetailsFromTapAsync(sender);
    }

    private async void OnTop10CardTapped(object sender, EventArgs e)
    {
        await OpenDetailsFromTapAsync(sender);
    }

    // -------------------- HELPERS --------------------

    private async Task OpenDetailsFromTapAsync(object sender)
    {
        try
        {
            _dramaService ??= Application.Current?.Handler?.MauiContext?.Services.GetService<DramaService>();
            if (_dramaService == null)
                throw new Exception("DramaService não disponível.");

            // TapGestureRecognizer é o sender; o BindingContext dele é o item do CollectionView
            if (sender is not TapGestureRecognizer tgr)
                return;

            if (tgr.BindingContext is not DramaService.CardItemVm card)
                return;

            var dramaId = (card.DramaId ?? "").Trim();
            if (string.IsNullOrWhiteSpace(dramaId))
                return;

            var drama = await _dramaService.GetDramaByIdAsync(dramaId);
            if (drama == null)
                return;

            await Navigation.PushAsync(new DramaDetailsPage(drama));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
}
