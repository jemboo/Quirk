using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quirk.UI.W.Core.Contracts.Services;
using Quirk.UI.W.Core.Services;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Quirk.UI.W.ViewModels;

public partial class ProjectsViewModel : ObservableRecipient
{
    public ProjectsViewModel(Storage.IProjectDataStore projectDataStore)
    {
        _projectDataStore = projectDataStore;

        FindProjectsCommand = new RelayCommand(
            FindProjects,
            CanFindProjects
            );

        PathProjectsRoot = "";
    }


    private readonly Storage.IProjectDataStore _projectDataStore;

    public ICommand FindProjectsCommand
    {
        get; set;
    }

    private async void FindProjects()
    {
        var yab = 4;
        var res = await _projectDataStore.GetProject(@"C:\Quirk", "Shc_064");
        var qp = Result.ExtractOrThrow(res);
        PathProjectsRoot = "Dats cool";
    }

    private bool CanFindProjects()
    {
        // Implement the condition to enable/disable the command
        return true;
    }


    [ObservableProperty]
    private string _pathProjectsRoot;

}
