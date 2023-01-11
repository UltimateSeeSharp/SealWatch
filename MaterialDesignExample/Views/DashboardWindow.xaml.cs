using SealWatch.Wpf.ViewModels;
using System.Windows;

namespace SealWatch.Wpf.Views;

public partial class DashboardWindow : Window
{
    public DashboardWindow(DashboardWindowViewModel dashboardWindowViewModel)
    {
        InitializeComponent();
        DataContext = dashboardWindowViewModel;
    }
}