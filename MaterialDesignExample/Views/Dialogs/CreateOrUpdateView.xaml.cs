using SealWatch.Wpf.ViewModels.Dialogs;
using System.Windows;

namespace SealWatch.Wpf.Views.Dialogs;

public partial class CreateOrUpdateView : Window
{
    public CreateOrUpdateView(CreateOrUpdateViewModel createOrUpdateViewModel)
    {
        InitializeComponent();
        DataContext = createOrUpdateViewModel;
        createOrUpdateViewModel.CloseWindow += (s, e) => this.Close();
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        this.DragMove();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        (DataContext as CreateOrUpdateViewModel)!.Loaded();
    }
}