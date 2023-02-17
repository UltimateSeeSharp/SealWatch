using SealWatch.Wpf.Config;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Views;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Text;
using System.Windows.Media.Animation;
using System.Threading;
using System.Threading.Tasks;

namespace SealWatch.Wpf.ViewModels;

public class SplashScreenViewModel : BaseViewModel
{
    private DispatcherTimer _dispatcherTimer = new();
    public event EventHandler? Close;

    public SplashScreenViewModel(AppSettings appSettings)
	{
        AppVersion = appSettings.AppVersion;

        _dispatcherTimer.Tick += new EventHandler(DispatcherTimerTick);
        _dispatcherTimer.Interval = new TimeSpan(0, 0, appSettings.SplashScreenTime);
        _dispatcherTimer.Start();
    }

    private string _appVersion = String.Empty;
    public string AppVersion
    {
        get => _appVersion;
        set
        {
            if (_appVersion == value)
                return;

            _appVersion = value;
            OnPropertyChanged();
        }
    }

    public void MouseLeftDown(Action action)
    {
        action.Invoke();
    }

    public void DispatcherTimerTick(object? sender, EventArgs e)
    {
        var window = Bootstrapper.Resolve<DashboardWindow>();
        window.Show();
        Close!.Invoke(null, EventArgs.Empty);
    }
}
