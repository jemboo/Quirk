using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace Quirk.UI.W.ViewModels;

public partial class BlankViewModel : ObservableRecipient
{
    public BlankViewModel()
    {
        // DeleteCommand = new RelayCommand(ExecuteMyCommand, CanExecuteMyCommand);
        //SwitchThemeCommand = new RelayCommand<ElementTheme>(
        // async (param) =>
        // {
        //     if (ElementTheme != param)
        //     {
        //         ElementTheme = param;
        //         await _themeSelectorService.SetThemeAsync(param);
        //     }
        // });
        DeleteCommand = new RelayCommand(
            //async () =>
            //{
            //    await Task.Run(() =>
            //    {
            //        ExecuteMyCommand();
            //    });
            //},
            ExecuteMyCommand,
            CanExecuteMyCommand
            ) ;

        _pathWorkspaceRoot = "";
    }

    public ICommand DeleteCommand
    {
        get; set;
    }


    private async void ExecuteMyCommand()
    {

        //await Task.Run(() => { 
        //    Debug.WriteLine("MyCommand executed!"); 

        //});

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
            Debug.WriteLine($"tada: {folder.Path}");
            PathWorkspaceRoot = folder.Path;
        }
    }

    private bool CanExecuteMyCommand()
    {
        // Implement the condition to enable/disable the command
        return true;
    }

    [ObservableProperty]
    private string _pathWorkspaceRoot;
}
