using DramaBox.Models;
using DramaBox.Services;

namespace DramaBox.Views;

public partial class LibraryPage : ContentPage
{
    private readonly DramaService _service;

    // Bindings usados no XAML:
    public string HeroCoverUrl { get; private set; } = "";
    public string HeroTitle { get; private set; } = "";
    public string HeroSubtitle { get; private set; } = "";

    public IList<Drama> LikedItems { get; private set; } = new List<Drama>();

    private Drama? _heroDrama;

    public LibraryPage()
    {
        InitializeComponent();

        _service = Application.Current?.Handler?.MauiContext?.Services.GetService<DramaService>()
                   ?? throw new Exception("DramaService não disponível.");

        BindingContext = this;
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
            // Por enquanto: biblioteca = catálogo oficial (mock), depois você troca por likes do usuário
            var all = await _service.GetOfficialSeriesAsync();

            // pega hero
            _heroDrama = all.FirstOrDefault();

            if (_heroDrama != null)
            {
                // monta hero
                HeroCoverUrl = _heroDrama.CoverUrl;
                HeroTitle = _heroDrama.Title;

                // exemplo: "Retomar • EP 2 • 1:18 restante" (mock)
                // depois você pode salvar lastPlayed em /users/{uid}/progress/...
                HeroSubtitle = "Retomar • EP 2 • 1:18 restante";
            }
            else
            {
                HeroCoverUrl = "";
                HeroTitle = "Sem séries";
                HeroSubtitle = "Adicione conteúdo no Firebase.";
            }

            // lista (se quiser excluir o hero: all.Skip(1).ToList())
            LikedItems = all.ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(HeroCoverUrl));
                OnPropertyChanged(nameof(HeroTitle));
                OnPropertyChanged(nameof(HeroSubtitle));
                OnPropertyChanged(nameof(LikedItems));
            });
        }
        catch
        {
            _heroDrama = null;
            HeroCoverUrl = "";
            HeroTitle = "Erro";
            HeroSubtitle = "Não foi possível carregar.";

            LikedItems = new List<Drama>();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(HeroCoverUrl));
                OnPropertyChanged(nameof(HeroTitle));
                OnPropertyChanged(nameof(HeroSubtitle));
                OnPropertyChanged(nameof(LikedItems));
            });
        }
    }

    // -------------------- HANDLERS DO XAML --------------------

    // botão lua/sol
    private void OnThemeClicked(object sender, EventArgs e)
    {
        // Alterna Light/Dark para teste rápido
        var current = Application.Current?.UserAppTheme ?? AppTheme.Unspecified;
        Application.Current!.UserAppTheme = current == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
    }

    // HERO: Continuar
    private async void OnContinueClicked(object sender, EventArgs e)
    {
        if (_heroDrama == null) return;

        try
        {
            var eps = await _service.GetEpisodesAsync(_heroDrama.Id, "s1");
            var ep = eps.FirstOrDefault();
            if (ep == null) return;

            await Navigation.PushAsync(new PlayerPage(_heroDrama, ep));
        }
        catch
        {
            // ignore
        }
    }

    // HERO: Curtir (mock)
    private async void OnLikeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Curtir", "Curtiu (mock). Depois ligamos em /users/{uid}/likes.", "OK");
    }

    // HERO: Share/link (mock)
    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (_heroDrama == null) return;
        await DisplayAlert("Link", $"Link (mock) para: {_heroDrama.Title}", "OK");
    }

    // GRID: tap no card curtido
    private async void OnLikedTapped(object sender, TappedEventArgs e)
    {
        // seu XAML passa CommandParameter="{Binding .}"
        var drama = e.Parameter as Drama;
        if (drama == null) return;

        await Navigation.PushAsync(new DramaDetailsPage(drama));
    }
}
