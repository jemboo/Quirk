using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Quirk.UI.W.Contracts.ViewModels;
using Quirk.UI.W.Core.Contracts.Services;
using Quirk.UI.W.Core.Models.Workspace;
using Quirk.UI.W.Core.Services;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Quirk.UI.W.ViewModels;

public partial class ProjectsViewModel : ObservableRecipient, INavigationAware
{
    public ProjectsViewModel(Storage.IProjectDataStore projectDataStore)
    {
        ProjectVms = new ObservableCollection<ProjectVm>();
        _projectDataStore = projectDataStore;

        FindProjectsCommand = new RelayCommand(
            FindProjects,
            CanFindProjects
            );

        PathProjectsRoot = "";
        StatusMessage = "Select a folder";
    }


    private readonly Storage.IProjectDataStore _projectDataStore;


    public ICommand FindProjectsCommand
    {
        get; set;
    }

    //private async void FindProjects()
    //{
    //    var yab = 4;
    //    var res = await _projectDataStore.GetProject(@"C:\Quirk", "Shc_064");
    //    var qp = Result.ExtractOrThrow(res);
    //    PathProjectsRoot = "Dats cool";
    //}
    public async void OnNavigatedTo(object parameter)
    {
        StatusMessage = "Select a folder";
        PathProjectsRoot = "";
    }


    private async void FindProjects()
    {
        FolderPicker fileOpenPicker = new()
        {
            ViewMode = PickerViewMode.Thumbnail
        };
        var windowHandle = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(fileOpenPicker, windowHandle);
        var folder = await fileOpenPicker.PickSingleFolderAsync();

        if (folder != null)
        {
            //CfgPlexes.Clear();
            PathProjectsRoot = folder.Path;
            //Instruction = "Select a project";

            var projectsFound = await _projectDataStore.GetAllProjectsAsync(PathProjectsRoot);
            if (projectsFound.IsError) 
            {
                StatusMessage = $"Error: {projectsFound.ErrorValue}";
            }
            else
            {
                StatusMessage = $"ProjectsFound: {projectsFound.ResultValue.Length}";
                foreach (var item in projectsFound.ResultValue)
                {
                   ProjectVms.Add(new ProjectVm());
                }
            }
        }
    }



    private bool CanFindProjects()
    {
        // Implement the condition to enable/disable the command
        return true;
    }

    public void OnNavigatedFrom()
    {
    
    }
    public void EnsureItemSelected()
    {
        SelectedProject ??= ProjectVms?.FirstOrDefault();
    }


    [ObservableProperty]
    private string _pathProjectsRoot;

    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private ProjectVm? _selectedProject;

    [ObservableProperty]
    private ObservableCollection<ProjectVm> _projectVms;


}
