using SealWatch.Wpf;
using System.Windows;
using SplashScreen = SealWatch.Wpf.Views.SplashScreen;

namespace MaterialDesignExample;

public partial class App : Application
{
    public App()
    {

    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Bootstrapper.Start();

        MainWindow = Bootstrapper.Resolve<SplashScreen>();
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Bootstrapper.Stop();
    }
}