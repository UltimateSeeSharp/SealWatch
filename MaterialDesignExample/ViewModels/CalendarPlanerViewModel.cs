using SealWatch.Code.CutterLayer;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.Services;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace SealWatch.Wpf.ViewModels;

/// <summary>
/// Displays calendars to show all future maintenance
/// dates of a specific cutter that can be selected
/// </summary>
public class CalendarPlanerViewModel : BaseViewModel
{
    private readonly ICutterAccessLayer _cutterAccessLayer;
    private readonly ICalendarService _sealMonitorService;
    private List<Calendar>? _calendars;

    public CalendarPlanerViewModel(ICutterAccessLayer cutterAccessLayer, ICalendarService calendarService)
    {
        _cutterAccessLayer = cutterAccessLayer;
        _sealMonitorService = calendarService;
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

            var failureDates = GetFailureDates(SelectedCutter);
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

    public List<DateTime> GetFailureDates(AnalysedCutterDto cutter)
    {
        List<DateTime> failureDates = new();
        DateTime endDate = cutter.MillingStart.AddMonths((int)(cutter.MillingDuration_y * 12));

        do
        {
            var startDate = failureDates.Count == 0 ? cutter.MillingStart : failureDates.Last();
            var failureDate = CalcFailureDate(startDate, cutter.WorkDays, cutter.MillingPerDay_h, cutter.LifeSpan_h);
            failureDates.Add(failureDate);
        }
        while (failureDates.Last() < endDate);

        var invalidDates = failureDates.Where(x => x == DateTime.MinValue || x == DateTime.MaxValue);
        if (invalidDates.Any())
            Log.Error("AnalyseService - GetFailureDates | Date(s) is MinValue/MaxValue");

        return failureDates;
    }

    public DateTime CalcFailureDate(DateTime millingStart, int workDays, double millingPerDay, double lifespan)
    {
        DateTime start = millingStart;

        while (lifespan > 0 && lifespan > millingPerDay * workDays)
        {
            lifespan -= (millingPerDay * workDays);
            start = start.AddDays(7);
        }

        if (lifespan is not 0)
        {
            var left = lifespan / millingPerDay;
            start = start.AddDays(left);
        }

        if (start == DateTime.MinValue || start == DateTime.MaxValue)
            Log.Error("AnalyseService - ClacFailureDate | Date is MinValue/MaxValue");

        return start;
    }
}