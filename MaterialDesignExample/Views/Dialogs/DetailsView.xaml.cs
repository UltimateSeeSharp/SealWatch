using SealWatch.Wpf.ViewModels.Dialogs;
using System.Windows;

namespace SealWatch.Wpf.Views.Dialogs;

public partial class DetailsView : Window
{
    public DetailsView(DetailsViewModel detailsViewModel)
    {
        InitializeComponent();
        DataContext = detailsViewModel;
        detailsViewModel.CloseWindow += (_, _) => this.Close();
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        this.DragMove();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        (DataContext as DetailsViewModel)!.Loaded();
    }
}