using Microsoft.Extensions.Logging;
using SealWatch.Wpf.Custom;
using SealWatch.Wpf.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SealWatch.Wpf.Views;

public partial class ProjectsView : UserControl
{
    public ProjectsView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && (listBox.ItemsSource as IEnumerable<CutterCard>)!.Count() > 0)
        {
            var cutterCards = listBox.ItemsSource as IEnumerable<CutterCard>;

            var selectedCutter = cutterCards!.FirstOrDefault(x => x.IsMouseOver)?.Cutter;
            if (selectedCutter is null) 
                return;

            (DataContext as ProjectViewModel)!.UpdateSelectedCutter(selectedCutter);
        }
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        (DataContext as ProjectViewModel)!.Loaded();
    }
}
