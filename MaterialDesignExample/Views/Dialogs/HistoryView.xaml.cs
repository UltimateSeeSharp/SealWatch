using SealWatch.Wpf.ViewModels.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace SealWatch.Wpf.Views.Dialogs;

public partial class HistoryView : Window
{
    public HistoryView(HistoryViewModel historyViewModel)
    {
        InitializeComponent();
        DataContext = historyViewModel;
        historyViewModel.CloseWindow += (_, _) => this.Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        this.DragMove();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        (DataContext as HistoryViewModel)!.Loaded();
    }
}