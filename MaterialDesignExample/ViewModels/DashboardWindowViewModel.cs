using SealWatch.Code.CutterLayer;
using SealWatch.Wpf.Config;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Views;
using SealWatch.Wpf.Views.New;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SealWatch.Wpf.ViewModels;

/// <summary>
/// Acts as an envelope for all Views(UserControls)
/// All UserControls get managed and loaded here
/// </summary>
public class DashboardWindowViewModel : BaseViewModel
{
    private readonly ProjectViewModel _projectsViewModel;
    private readonly AnalyticViewModel _analyticViewModel;
    private readonly CalendarPlanerViewModel _calendarPlanerViewModel;
    private readonly AppSettings _appSettings;

    public DashboardWindowViewModel(
        ProjectViewModel projectsOverviewViewModel, 
        AnalyticViewModel analyticViewModel, 
        CalendarPlanerViewModel calendarPlanerViewModel,
        AppSettings appSettings)
    {
        _projectsViewModel = projectsOverviewViewModel;
        _analyticViewModel = analyticViewModel;
        _calendarPlanerViewModel = calendarPlanerViewModel;
        _appSettings = appSettings;

        ProjectsView = new ProjectsView(_projectsViewModel);
        AnalyseView = new Views.New.AnalyticView(_analyticViewModel);
        CalendarView = new CalendarPlanerView(_calendarPlanerViewModel);
    }

    public UserControl ProjectsView { get; set; }

    public UserControl AnalyseView { get; set; }
    
    public UserControl CalendarView { get; set; }

    public string AppNameAndVersion => "Seal Watch" + " " + _appSettings.AppVersion;

    public string CurrentUser => Environment.UserName;

    public ICommand CreaditCommand => new DelegateCommand()
    {
        CommandAction = () => MessageBox.Show("https://www.flaticon.com/de/autoren/freepik", "Icon credit")
    };
}