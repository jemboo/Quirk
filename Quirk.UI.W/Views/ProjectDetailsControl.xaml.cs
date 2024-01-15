using CommunityToolkit.WinUI.UI.Controls;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Quirk.UI.W.ComponentVms;
using Quirk.UI.W.Core.Models;
using Quirk.UI.W.Core.Models.Workspace;

namespace Quirk.UI.W.Views;

public sealed partial class ProjectDetailsControl : UserControl
{
    public ProjectVm? ProjectVm
    {
        get => GetValue(ProjectProperty) as ProjectVm;
        set => SetValue(ProjectProperty, value);
    }

    public static readonly DependencyProperty ProjectProperty = 
            DependencyProperty.Register(
                    "ProjectVm", 
                    typeof(ProjectVm), 
                    typeof(ProjectDetailsControl), 
                    new PropertyMetadata(null, OnProjectVmPropertyChanged));

    public ProjectDetailsControl()
    {
        InitializeComponent();
    }

    private static void OnProjectVmPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ProjectDetailsControl control)
        {
            //control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }

    private void dg_Sorting(object sender, DataGridColumnEventArgs e)
    {
        var qua = ProjectVm?.QuirkWorldLineVms;
        if ((e.Column.Tag).ToString() == "QuirkModelType")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.QuirkModelType ascending
                    select item
                    );
        }
        if ((e.Column.Tag).ToString() == "QuirkWorldLineId")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.QuirkWorldLineId ascending
                    select item
                    );
        }
        if ((e.Column.Tag).ToString() == "ReplicaNum")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.ReplicaNum ascending
                    select item
                    );
        }
        if ((e.Column.Tag).ToString() == "Val2")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.Val2 ascending
                    select item
                    );
        }
        if ((e.Column.Tag).ToString() == "Val3")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.Val3 ascending
                    select item
                    );
        }
        if ((e.Column.Tag).ToString() == "Val4")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.Val4 ascending
                    select item
                    );
        }
        if ((e.Column.Tag).ToString() == "Val5")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.Val5 ascending
                    select item
                    );
        }
        if ((e.Column.Tag).ToString() == "Val6")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.Val6 ascending
                    select item
                    );
        }
        if ((e.Column.Tag).ToString() == "Val7")
        {
            var qua2 = dg_worldLines.ItemsSource =
                new ObservableCollection<QuirkWorldLineVm>(
                    from item in qua
                    orderby item.Val7 ascending
                    select item
                    );
        }

    }
}
