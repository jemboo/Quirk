using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Quirk.UI.W.Core.Models;
using Quirk.UI.W.ViewModels;

namespace Quirk.UI.W.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class DataGridPage : Page
{
    public DataGridViewModel ViewModel
    {
        get;
    }

    public DataGridPage()
    {
        ViewModel = App.GetService<DataGridViewModel>();
        InitializeComponent();
    }

    private void dg_Sorting(object sender, DataGridColumnEventArgs e)
    {
        var qua = ViewModel.Source;
        if ((e.Column.Tag).ToString() == "OrderID")
        {
            var qua2 = theDataGrid.ItemsSource =
                new ObservableCollection<SampleOrder>(
                    from item in qua
                        orderby item.OrderID ascending
                        select item
                    );

        }
    }

    }
