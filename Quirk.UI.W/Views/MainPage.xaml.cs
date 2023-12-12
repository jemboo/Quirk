using Microsoft.UI.Xaml.Controls;

using Quirk.UI.W.ViewModels;

namespace Quirk.UI.W.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
