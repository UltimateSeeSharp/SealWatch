using SealWatch.Code.ProjectLayer;
using SealWatch.Wpf;
using SealWatch.Wpf.ViewModels.Dialogs;
using SealWatch.Wpf.Views;
using System.Windows;

namespace MaterialDesignExample;

public partial class App : Application
{
    public App()
    {
        var dvm = new DetailsViewModel(new ProjectAccessLayer());
        dvm.Project = new ProjectDetailDto();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Bootstrapper.Start();

        MainWindow = Bootstrapper.Resolve<SealWatch.Wpf.Views.SplashScreen>();
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Bootstrapper.Stop();
    }
}