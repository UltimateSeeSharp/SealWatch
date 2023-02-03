using SealWatch.Code.CutterLayer;
using SealWatch.Wpf.Config;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Views;
using System;
using System.Windows.Controls;

namespace SealWatch.Wpf.ViewModels;

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

        Projects = new ProjectsView(_projectsViewModel);
        Analyse = new AnalyticView(_analyticViewModel);
        AnalyseOld = new CalendarPlanerView(_calendarPlanerViewModel);
    }

    public UserControl Projects { get; set; }

    public UserControl Analyse { get; set; }
    
    public UserControl AnalyseOld { get; set; }

    public string AppNameAndVersion => "Seal Watch" + " " + _appSettings.AppVersion;

    public string CurrentUser => Environment.UserName;
}
