using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quirk.UI.W.Contracts.ViewModels;
using Quirk.UI.W.Core.Contracts.Services;
using Quirk.UI.W.Core.Models.Workspace;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Quirk.UI.W.ViewModels;

public partial class ConfigsViewModel : ObservableRecipient, INavigationAware
{
    public ConfigsViewModel(IWorkspaceDataService workspaceDataService)
    {
        CfgPlexes = new ObservableCollection<CfgPlexVm>();
        _workspaceDataService = workspaceDataService;

        FindWorkspaceCommand = new RelayCommand(
            FindWorkspace,
            CanFindWorkspace
            );

        PathWorkspaceRoot = "";

        Instruction = "Select a workspace folder";
    }

    private readonly IWorkspaceDataService _workspaceDataService;

    [ObservableProperty]
    private CfgPlexVm? _selected;

    partial void OnSelectedChanged(CfgPlexVm? value)
    {
        if (value != null)
        {
            var task = Task.Run(async () => await _workspaceDataService.GetCfgPlexDetails(PathWorkspaceRoot, value));
            var cfgPlexVmUpdated = task.Result;
            value.CopyValuesFrom(cfgPlexVmUpdated);
        }
    }

    [ObservableProperty]
    private ObservableCollection<CfgPlexVm> _cfgPlexes;

    public async void OnNavigatedTo(object parameter)
    {
        CfgPlexes.Clear();

        // TODO: Replace with real data.
        var data = await _workspaceDataService.GetListDetailsDataAsync();

        foreach (var item in data)
        {
            CfgPlexes.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void EnsureItemSelected()
    {
        Selected ??= CfgPlexes?.FirstOrDefault();
    }


    public ICommand FindWorkspaceCommand
    {
        get; set;
    }


    private async void FindWorkspace()
    {
        FolderPicker fileOpenPicker = new()
        {
            ViewMode = PickerViewMode.Thumbnail,
            FileTypeFilter = { ".jpg", ".jpeg", ".png", ".gif" },
        };
        nint windowHandle = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(fileOpenPicker, windowHandle);
        var folder = await fileOpenPicker.PickSingleFolderAsync();

        if (folder != null)
        {
            CfgPlexes.Clear();
            PathWorkspaceRoot = folder.Path;
            Instruction = "Select a project";

            var plexesFound = await _workspaceDataService.GetCfgPlexesInWorkspace(PathWorkspaceRoot);

            foreach (var item in plexesFound)
            {
                CfgPlexes.Add(item);
            }
        }
    }

    private bool CanFindWorkspace()
    {
        // Implement the condition to enable/disable the command
        return true;
    }

    [ObservableProperty]
    private string _pathWorkspaceRoot;


    [ObservableProperty]
    private string _instruction;

}
