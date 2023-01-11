 using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SealWatch.Wpf.Views;

public partial class SplashScreen : Window
{
    DispatcherTimer _dispatcherTimer = new();

    public SplashScreen()
    {
        InitializeComponent();

        _dispatcherTimer.Tick += new EventHandler(_dispatcherTimer_Tick);
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
        _dispatcherTimer.Start();
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        this.DragMove();
    }

    private void _dispatcherTimer_Tick(object? sender, EventArgs e)
    {
        var window = Bootstrapper.Resolve<DashboardWindow>();
        window.Show();
        this.Close();
    }
}