using Microsoft.UI.Xaml.Controls;

using Quirk.UI.W.ViewModels;

namespace Quirk.UI.W.Views;

public sealed partial class BlankPage : Page
{
    public BlankViewModel ViewModel
    {
        get;
    }

    public BlankPage()
    {
        ViewModel = App.GetService<BlankViewModel>();

        InitializeComponent();
    }
}
