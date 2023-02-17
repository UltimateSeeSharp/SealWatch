using SealWatch.Wpf.ViewModels;
using System.Windows.Controls;

namespace SealWatch.Wpf.Views.New;

public partial class AnalyticView : UserControl
{
    public AnalyticView(AnalyticViewModel analyticViewModel)
    {
        InitializeComponent();
        DataContext = analyticViewModel;
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        (DataContext as AnalyticViewModel)!.Loaded();
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is null)
            return;

        var selectedCutter = (sender as DataGrid)!.SelectedItem;
        if (selectedCutter is null)
            return;

        (DataContext as AnalyticViewModel)!.OrderCommand.Execute(0);
    }
}
