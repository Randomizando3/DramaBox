using DramaBox.Models;
using DramaBox.Services;

namespace DramaBox.Views;

public partial class DramaDetailsPage : ContentPage
{
    private readonly Drama _drama;
    private readonly DramaService _service;

    public DramaDetailsPage(Drama drama)
    {
        InitializeComponent();

        _drama = drama;

        _service = Application.Current?.Handler?.MauiContext?.Services.GetService<DramaService>()
                   ?? throw new Exception("DramaService não disponível.");

        Cover.Source = _drama.CoverUrl;
        TitleLbl.Text = _drama.Title;
        GenreLbl.Text = _drama.Genre;
        SynLbl.Text = _drama.Synopsis;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var eps = await _service.GetEpisodesAsync(_drama.Id);
            Episodes.ItemsSource = eps;
        }
        catch
        {
            Episodes.ItemsSource = Array.Empty<Episode>();
        }
    }

    private async void OnSelectEp(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection?.FirstOrDefault() is Episode ep)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Navigation.PushAsync(new PlayerPage(_drama, ep));
        }
    }
}
