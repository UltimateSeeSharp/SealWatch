using SealWatch.Code.CutterLayer;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.ProjectLayer;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SealWatch.Wpf.ViewModels;

public class CalendarPlanerViewModel : BaseViewModel
{
    private readonly ICutterAccessLayer _cutterAccessLayer;
    private readonly ISealMonitorService _sealMonitorService;

    private List<Calendar>? _calendars;

    public CalendarPlanerViewModel(ICutterAccessLayer cutterAccessLayer, ISealMonitorService sealMonitorService)
    {
        _cutterAccessLayer = cutterAccessLayer;
        _sealMonitorService = sealMonitorService;
    }

    public void Loaded(List<Calendar> calendars)
    {
        RefreshProjects();

        _calendars = calendars;
        _sealMonitorService.InitializeCalendars(DateTime.Now, _calendars);
    }

    public ObservableCollection<CutterAnalyseDto>? Cutters { get; set; }

    private CutterAnalyseDto? _selectedCutter;
    public CutterAnalyseDto? SelectedCutter
    {
        get => _selectedCutter;
        set
        {
            if (_selectedCutter == value) return;
            _selectedCutter = value;
            OnPropertyChanged();

            if (SelectedCutter is null)
                return;

            _sealMonitorService.DrawSelected(_calendars!, SelectedCutter.WorkDays, SelectedCutter.MillingPerDay_h, SelectedCutter.MillingDuration_y, SelectedCutter.LifeSpan_h, SelectedCutter.MillingStart);
        }
    }

    private string _cutterSearchText;
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
        Cutters = new(_cutterAccessLayer.GetAnalyticData(_cutterSearchText));
        NoCuttersAvailable = Cutters.Count > 0 ? false : true;
        OnPropertyChanged(nameof(Cutters));
    }
}