using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Quirk.UI.W.ComponentVms;
using Quirk.UI.W.Core.Models;
using Quirk.UI.W.Core.Models.Workspace;

namespace Quirk.UI.W.Views;

public sealed partial class ProjectDetailsControl : UserControl
{
    public ProjectVm? Project
    {
        get => GetValue(ProjectProperty) as ProjectVm;
        set => SetValue(ProjectProperty, value);
    }

    public static readonly DependencyProperty ProjectProperty = 
            DependencyProperty.Register(
                    "Project", 
                    typeof(ProjectVm), 
                    typeof(ProjectDetailsControl), 
                    new PropertyMetadata(null, OnProjectPropertyChanged));

    public ProjectDetailsControl()
    {
        InitializeComponent();
    }

    private static void OnProjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ProjectDetailsControl control)
        {
            //control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
