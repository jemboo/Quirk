using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;

using Quirk.UI.W.ViewModels;

namespace Quirk.UI.W.Views;

public sealed partial class ProjectsPage : Page
{
    public ProjectsViewModel ViewModel
    {
        get;
    }

    public ProjectsPage()
    {
        ViewModel = App.GetService<ProjectsViewModel>();
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
