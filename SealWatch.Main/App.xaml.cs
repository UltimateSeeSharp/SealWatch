using System.Windows;

namespace SealWatch.Main;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Bootstrapper.Start();

        MainWindow = Bootstrapper.Resolve<MainWindow>();
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Bootstrapper.Stop();
    }
}