using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DramaBox.Services;

namespace DramaBox.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly DramaService _service;

    [ObservableProperty] private bool isBusy;

    public ObservableCollection<DramaService.HeroItemVm> HeroItems { get; } = new();
    public ObservableCollection<DramaService.CardItemVm> NewItems { get; } = new();
    public ObservableCollection<DramaService.CardItemVm> Top10Series { get; } = new();

    public HomeViewModel(DramaService service)
    {
        _service = service;
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var data = await _service.GetOfficialHomeAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                HeroItems.Clear();
                NewItems.Clear();
                Top10Series.Clear();

                foreach (var x in data.Hero) HeroItems.Add(x);
                foreach (var x in data.NewItems) NewItems.Add(x);
                foreach (var x in data.Top10) Top10Series.Add(x);
            });
        }
        finally
        {
            IsBusy = false;
        }
    }
}
