using SealwatchTest.ViewModel;
using System.Windows;

namespace SealwatchTest.View;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}