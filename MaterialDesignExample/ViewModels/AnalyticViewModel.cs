using LiveCharts.Wpf;
using SealWatch.Code.CutterLayer;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SealWatch.Wpf.ViewModels;

public class AnalyticViewModel : BaseViewModel
{
    private readonly ICutterAccessLayer _cutterAccessLayer;
    private readonly IUserInputService _userInputService;
    private readonly ICoorporateDesignService _coorporateDesignService;
    private readonly IGraphsService _graphsService;

    private List<CutterAnalyseDto> _cutters = new();
    private int _dayFilter = 360;

    public AnalyticViewModel(
        ICutterAccessLayer cutterAccessLayer, 
        IUserInputService userInputService, 
        ICoorporateDesignService coorporateDesignService,
        IGraphsService graphsService)
    {
        _cutterAccessLayer = cutterAccessLayer;
        _userInputService = userInputService;
        _coorporateDesignService = coorporateDesignService;
        _graphsService = graphsService;
    }

    public void Loaded()
    {
        RefreshCutters();
    }

    public ICommand OrderCommand => new DelegateCommand()
    {
        CanExecuteFunc = () => SelectedCutter is not null,
        CommandAction = () =>
        {
            if (_userInputService.UserPopupConfirmed("Bestellen"))
            {
                _cutterAccessLayer.Order(SelectedCutter!.Id);
                RefreshCutters();
            }
        }
    };

    public ICommand FilterCommand => new DelegateCommand()
    {
        ObjectCommandAction = (x) =>
        {
            _dayFilter = Convert.ToInt32(x);
            RefreshCutters();
            RefreshGraphs();
        }
    };

    private CutterAnalyseDto? _selectedCutter;
    public CutterAnalyseDto? SelectedCutter
    {
        get => _selectedCutter;
        set
        {
            if (_selectedCutter == value) return;
            _selectedCutter = value;
            OnPropertyChanged();

            RefreshGraphs();
        }
    }

    public ObservableCollection<CutterAnalyseDto> Cutters { get; set; } = new();

    private CartesianChart _orderedChart;
    public CartesianChart OrderedChart 
    {
        get => _orderedChart;
        set
        {
            if (_orderedChart == value)
                return;

            _orderedChart = value;
            OnPropertyChanged();
        }
    }

    private CartesianChart _locationChart;
    public CartesianChart LocationChart 
    {
        get => _locationChart;
        set
        {
            if (_locationChart == value)
                return;

            _locationChart = value;
            OnPropertyChanged();
        }
    }

    private string _cutterSearchText = string.Empty;
    public string CutterSearchText
    {
        get => _cutterSearchText;
        set
        {
            _cutterSearchText = value;
            OnPropertyChanged();

            RefreshCutters();
        }
    }

    private bool _noCuttersAvailable = false;
    public bool NoCuttersAvailable
    {
        get => _noCuttersAvailable;
        set
        {
            _noCuttersAvailable = value;
            OnPropertyChanged();
        }
    }

    private void RefreshCutters()
    {
        _cutters = _cutterAccessLayer.GetAnalyticData(_cutterSearchText);
        _cutters = _cutters.Where(x => x.DaysLeft <= _dayFilter).ToList();
        Cutters = new(_cutters);
        OnPropertyChanged(nameof(Cutters));

        if (Cutters.Count <= 0)
            NoCuttersAvailable = true;
        else
            NoCuttersAvailable = false;

        RefreshGraphs();
    }

    public void RefreshGraphs()
    {
        if (Cutters is null) 
            return;

        var cutters = Cutters.ToList();

        OrderedChart = _graphsService.GetOrderedChart(cutters, _dayFilter);
        OnPropertyChanged(nameof(OrderedChart));
        LocationChart = _graphsService.GetLocationChart(cutters, _dayFilter);
        OnPropertyChanged(nameof(LocationChart));
    }

    public void SelectedCutterChanged(CutterAnalyseDto cutter) => SelectedCutter = cutter;
}