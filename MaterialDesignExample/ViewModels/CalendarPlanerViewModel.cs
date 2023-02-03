using SealWatch.Code.CutterLayer;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.Services;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace SealWatch.Wpf.ViewModels;

public class CalendarPlanerViewModel : BaseViewModel
{
    private readonly ICutterAccessLayer _cutterAccessLayer;
    private readonly ICalendarService _sealMonitorService;
    private readonly AnalyseService _analyseService;
    private List<Calendar>? _calendars;

    public CalendarPlanerViewModel(ICutterAccessLayer cutterAccessLayer, ICalendarService sealMonitorService, AnalyseService analyseService)
    {
        _cutterAccessLayer = cutterAccessLayer;
        _sealMonitorService = sealMonitorService;
        _analyseService = analyseService;
    }

    public void Loaded(List<Calendar> calendars)
    {
        RefreshProjects();

        _calendars = calendars;
        _sealMonitorService.InitializeCalendars(DateTime.Now, _calendars);
    }

    public ObservableCollection<AnalysedCutterDto>? Cutters { get; set; }

    private AnalysedCutterDto? _selectedCutter;
    public AnalysedCutterDto? SelectedCutter
    {
        get => _selectedCutter;
        set
        {
            if (_selectedCutter == value) return;
            _selectedCutter = value;
            OnPropertyChanged();

            if (SelectedCutter is null)
                return;

            var failureDates = _analyseService.GetFailureDates(SelectedCutter);
            _sealMonitorService.DrawSelected(_calendars!, failureDates);
        }
    }

    private string _cutterSearchText = String.Empty;
    public string CutterSearchText
    {
        get => _cutterSearchText;
        set
        {
            _cutterSearchText = value;
            OnPropertyChanged();

            RefreshProjects();
        }
    }

    private bool _cuttersAvailable = false;
    public bool NoCuttersAvailable
    {
        get => _cuttersAvailable;
        set
        {
            _cuttersAvailable = value;
            OnPropertyChanged();
        }
    }

    private void RefreshProjects()
    {
        Cutters = new(_cutterAccessLayer.GetAnalysedCutters(search: _cutterSearchText));
        NoCuttersAvailable = Cutters.Count > 0 ? false : true;
        OnPropertyChanged(nameof(Cutters));
    }
}