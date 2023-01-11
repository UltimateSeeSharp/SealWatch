using SealWatch.Wpf.ViewModels.Dialogs;
using System.Windows;

namespace SealWatch.Wpf.Views.Dialogs;

public partial class CreateOrUpdateCutterView : Window
{
    public CreateOrUpdateCutterView(CreateOrUpdateCutterViewModel createOrUpdateCutterViewModel)
    {
        InitializeComponent();
        DataContext = createOrUpdateCutterViewModel;
        createOrUpdateCutterViewModel.CloseWindow += (s, e) => this.Close();
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        this.DragMove();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        (DataContext as CreateOrUpdateCutterViewModel)!.Loaded();
    }
}