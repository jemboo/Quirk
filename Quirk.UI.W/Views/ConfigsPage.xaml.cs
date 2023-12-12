using CommunityToolkit.WinUI.UI.Controls;

using Microsoft.UI.Xaml.Controls;

using Quirk.UI.W.ViewModels;

namespace Quirk.UI.W.Views;

public sealed partial class ConfigsPage : Page
{
    public ConfigsViewModel ViewModel
    {
        get;
    }

    public ConfigsPage()
    {
        ViewModel = App.GetService<ConfigsViewModel>();
        InitializeComponent();
    }

    private void OnViewStateChanged(object sender, ListDetailsViewState e)
    {
        if (e == ListDetailsViewState.Both)
        {
            ViewModel.EnsureItemSelected();
        }
    }
}
