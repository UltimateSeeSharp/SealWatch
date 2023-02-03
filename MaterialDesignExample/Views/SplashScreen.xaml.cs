using SealWatch.Wpf.Config;
using System;
using System.Windows;
using System.Windows.Threading;

namespace SealWatch.Wpf.Views;

public partial class SplashScreen : Window
{
    private readonly AppSettings _appSettings;

    DispatcherTimer _dispatcherTimer = new();

    public SplashScreen(AppSettings appSettings)
    {
        InitializeComponent();

        _appSettings = appSettings;

        _dispatcherTimer.Tick += new EventHandler(_dispatcherTimer_Tick);
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 50);
        _dispatcherTimer.Start();
    }

    public string AppVersion => _appSettings.AppVersion;

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