using LiveCharts.Wpf;
using SealWatch.Code.CutterLayer;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.Enums;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SealWatch.Wpf.ViewModels;

/// <summary>
/// Offers graphs and information about the cutter
/// location and future maintenance dates
/// </summary>
public class AnalyticViewModel : BaseViewModel
{
    private readonly ICutterAccessLayer _cutterAccessLayer;
    private readonly IUserInputService _userInputService;
    private readonly IDesignService _coorporateDesignService;
    private readonly IGraphsService _graphsService;

    private List<AnalysedCutterDto> _cutters = new();
    private Timeframe _timeframe = Timeframe.Year;

    public AnalyticViewModel(
        ICutterAccessLayer cutterAccessLayer, 
        IUserInputService userInputService, 
        IDesignService coorporateDesignService,
        IGraphsService graphsService)
    {
        _cutterAccessLayer = cutterAccessLayer;
        _userInputService = userInputService;
        _coorporateDesignService = coorporateDesignService;
        _graphsService = graphsService;
    }

    public void Loaded() => RefreshCutters();

    public ICommand OrderCommand => new DelegateCommand()
    {
        CanExecuteFunc = () => SelectedCutter is not null,
        CommandAction = () =>
        {
            if (_userInputService.UserConfirmPopUp("Bestellen"))
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
            _timeframe = (Timeframe)Enum.Parse(typeof(Timeframe), x.ToString()!);
            RefreshCutters();
            RefreshGraphs();
        }
    };

    private AnalysedCutterDto? _selectedCutter;
    public AnalysedCutterDto? SelectedCutter
    {
        get => _selectedCutter;
        set
        {
            if (_selectedCutter == value) 
                return;

            _selectedCutter = value;
            OnPropertyChanged();

            RefreshGraphs();
        }
    }

    public ObservableCollection<AnalysedCutterDto> Cutters { get; set; } = new();

    private CartesianChart? _orderedChart;
    public CartesianChart? OrderedChart 
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

    private CartesianChart? _locationChart;
    public CartesianChart? LocationChart 
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
        Cutters = new(_cutterAccessLayer.GetAnalysedCutters(search: _cutterSearchText, timeframe: _timeframe));
        NoCuttersAvailable = Cutters.Count <= 0 ? true : false;

        OnPropertyChanged(nameof(Cutters));
        RefreshGraphs();
    }

    public void RefreshGraphs()
    {
        if (Cutters is null || Cutters.Count is 0) 
            return;

        OrderedChart = _graphsService.GetOrderedSealsChart(Cutters.ToList(), _timeframe);
        LocationChart = _graphsService.GetCutterLocationChart(Cutters.ToList());
    }

    public void SelectedCutterChanged(AnalysedCutterDto cutter) => SelectedCutter = cutter;
}