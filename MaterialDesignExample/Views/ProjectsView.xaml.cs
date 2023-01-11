using SealWatch.Wpf.ViewModels;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using SealWatch.Wpf.Custom;

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
            if (selectedCutter is null) return;
            (DataContext as ProjectViewModel)!.UpdateSelectedCutter(selectedCutter);
        }
    }
}
