using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Quirk.UI.W.Contracts.ViewModels;
using Quirk.UI.W.Core.Contracts.Services;
using Quirk.UI.W.Core.Models;

namespace Quirk.UI.W.ViewModels;

public partial class DataGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public DataGridViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _sampleDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public string Hdr => "Ralph";
    public Visibility Vis => Visibility.Visible;

    public void OnNavigatedFrom()
    {
    }
}
