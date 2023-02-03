using SealWatch.Wpf.ViewModels;
using System;
using System.Windows;

namespace SealWatch.Wpf.Views;

public partial class SplashScreen : Window
{
    private SplashScreenViewModel _splashScreenViewModel;

    public SplashScreen(SplashScreenViewModel splashScreenViewModel)
    {
        _splashScreenViewModel = splashScreenViewModel;
        _splashScreenViewModel.Close += (s, args) => this.Close();

        InitializeComponent();
        DataContext = _splashScreenViewModel;
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _splashScreenViewModel.MouseLeftDown(() =>
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        });
    }

    private void _dispatcherTimer_Tick(object? sender, EventArgs e)
    {
        _splashScreenViewModel.DispatcherTimerTick(sender, e);
    }
}