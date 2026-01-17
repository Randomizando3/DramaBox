using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DramaBox.Models;
using DramaBox.Services;

namespace DramaBox.ViewModels;

public partial class DiscoverViewModel : ObservableObject
{
    private readonly DramaService _service;

    [ObservableProperty] private bool isBusy;

    public ObservableCollection<Drama> Items { get; } = new();

    public DiscoverViewModel(DramaService service)
    {
        _service = service;
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var list = await _service.GetOfficialSeriesAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Items.Clear();
                foreach (var d in list) Items.Add(d);
            });
        }
        finally
        {
            IsBusy = false;
        }
    }
}
