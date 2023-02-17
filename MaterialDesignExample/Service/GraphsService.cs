using LiveCharts;
using LiveCharts.Wpf;
using SealWatch.Code.CutterLayer;
using SealWatch.Code.Enums;
using SealWatch.Wpf.Config;
using SealWatch.Wpf.Service.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace SealWatch.Wpf.Service;


/// <summary>
/// Generates graphs for AnalyticView
/// </summary>
public class GraphsService : IGraphsService
{
    private readonly IDesignService _coorporateDesignService;
    private readonly AppSettings _appSettings;

    public GraphsService(IDesignService coorporateDesignService, AppSettings appSettings)
    {
        _coorporateDesignService = coorporateDesignService;
        _appSettings = appSettings;
    }

    public CartesianChart GetOrderedSealsChart(List<AnalysedCutterDto> cutters, Timeframe timeframe = Timeframe.Week)
    {
        SeriesCollection? series = GetFailureDatesCollection(cutters, timeframe);
        if (series is null)
            Log.Error("GraphsService - GetOrderedSealsChart | SeriesCollection of chart was null");

        return new()
        {
            Series = series,
            AxisX = GetFailureDatesAxis(timeframe),
            AxisY = GetYAxis()
        };
    }

    public CartesianChart GetCutterLocationChart(List<AnalysedCutterDto> cutters)
    {
        SeriesCollection? series = GetCutterLocationCollection(cutters);
        if (series is null)
            Log.Error("GraphsService - GetCutterLocationChart | SeriesCollection of chart was null");

        return new()
        {
            Series = GetCutterLocationCollection(cutters),
            AxisX = GetCutterLocationAxis(cutters),
            AxisY = GetYAxis()
        };
    }

    private SeriesCollection? GetFailureDatesCollection(List<AnalysedCutterDto> cutters, Timeframe timeframe)
    {
        if (cutters is null)
        {
            Log.Error("GraphsService - GetFailureDatesCollection | Cutters were null");
            return null;
        }
        if (cutters.Count is 0)
        {
            Log.Error("GraphsService - GetFailureDatesCollection | Cutters had count 0");
            return null;
        }

        var orderedCutters = cutters.Where(x => x.SealOrdered).ToList();

        SeriesCollection collection = new()
        {
            new LineSeries()
            {
                Title = _appSettings.NotOrderedText,
                LineSmoothness = 0.5,
                Fill = Brushes.Transparent,
                Stroke = Brushes.LightGray,
                Values = new ChartValues<int>()
            },
            new LineSeries()
            {
                Title = _appSettings.OrderedText,
                LineSmoothness = 0.5,
                Fill = _coorporateDesignService.colGold60,
                Stroke = Brushes.Gold,
                Values = new ChartValues<int>()
            }
        };

        if (timeframe is Timeframe.Week)
        {
            for (int x = 0; x <= 7; x++)
            {
                collection[0].Values.Add(
                    cutters.Count(c => c.MillingStop.Date == DateTime.Now.AddDays(x).Date));
            }
            for (int x = 0; x <= 7; x++)
            {
                collection[1].Values.Add(
                    orderedCutters.Count(c => c.MillingStop.Date == DateTime.Now.AddDays(x).Date));
            }
        }
        else if (timeframe is Timeframe.Month)
        {
            for (int x = 0; x < 4; x++)
            {
                collection[0].Values.Add(
                    cutters.Count(c => c.MillingStop.Date >= DateTime.Now.AddDays(7 * x).Date
                                    && c.MillingStop.Date <= DateTime.Now.AddDays(7 * (x + 1)).Date));
            }
            for (int x = 0; x < 4; x++)
            {
                collection[1].Values.Add(
                    orderedCutters.Count(c => c.MillingStop.Date >= DateTime.Now.AddDays(7 * x).Date
                                           && c.MillingStop.Date <= DateTime.Now.AddDays(7 * (x + 1)).Date));
            }

            return collection;
        }
        else if (timeframe is Timeframe.Year)
        {
            for (int x = 0; x < 12; x++)
            {
                collection[0].Values.Add(cutters.Count(c => c.MillingStop.Month == DateTime.Now.AddMonths(x).Month
                                                         && c.MillingStop.Year == DateTime.Now.AddMonths(x).Year));
            }
            for (int x = 0; x < 12; x++)
            {
                collection[1].Values.Add(cutters.Count(c => c.SealOrdered
                                                         && c.MillingStop.Month == DateTime.Now.AddMonths(x).Month
                                                         && c.MillingStop.Year == DateTime.Now.AddMonths(x).Year));
            }
        }

        return collection;
    }
    private AxesCollection GetFailureDatesAxis(Timeframe timeframe)
    {
        Axis axis = new()
        {
            Labels = new List<string>(),
            Foreground = Brushes.Black,
            Separator = new Separator()
            {
                Step = 1,
                Stroke = _coorporateDesignService.colGrid
            }
        };

        if (timeframe is Timeframe.Week)
        {
            Enumerable.Range(0, 8).ToList().ForEach(x => axis.Labels.Add(DateTime.Now.AddDays(x).ToString("dd MMM")));
        }
        else if (timeframe is Timeframe.Month)
        {
            Enumerable.Range(0, 4).ToList().ForEach(x => axis.Labels.Add(
                    DateTime.Now.AddDays(7 * x).Date.ToString("dd.MM") + 
                    " - " + 
                    DateTime.Now.AddDays(7 * (x + 1)).Date.ToString("dd.MM")));
        }
        else if (timeframe is Timeframe.Year)
        {
            Enumerable.Range(0, 12).ToList().ForEach(x => axis.Labels.Add(DateTime.Now.AddMonths(x).ToString("MMM")));
        }

        return new AxesCollection() { axis };
    }

    private SeriesCollection? GetCutterLocationCollection(List<AnalysedCutterDto> cutters)
    {
        if (cutters is null)
        {
            Log.Error("GraphsService - GetCutterLocationCollection | Cutters were null");
            return null;
        }
        if (cutters.Count is 0)
        {
            Log.Error("GraphsService - GetCutterLocationCollection | Cutters had count 0");
            return null;
        }

        ColumnSeries columnSeries = new()
        {
            Title = "Standort",
            DataLabels = true,
            Values = new ChartValues<int>(),
            Fill = _coorporateDesignService.colGold160,
        };

        var locations = cutters.Select(x => x.Location).Distinct();

        if (locations.Count() > _appSettings.MaxGraphLocations)
            locations = locations.Take(_appSettings.MaxGraphLocations);

        foreach (var location in locations)
        {
            var cuttersAtLocation = cutters.Count(x => x.Location == location);
            columnSeries.Values.Add(cuttersAtLocation);
        }

        return new SeriesCollection { columnSeries };
    }
    private AxesCollection GetCutterLocationAxis(List<AnalysedCutterDto> cutters)
    {
        var locations = cutters.Select(x => x.Location).Distinct();

        if (locations.Count() > _appSettings.MaxGraphLocations)
            locations = locations.Take(_appSettings.MaxGraphLocations);

        var axis = new Axis()
        {
            Labels = locations.ToArray(),
            Foreground = Brushes.Black,
            Separator = new()
            {
                Step = 1,
                Stroke = _coorporateDesignService.colGrid
            }
        };

        return new AxesCollection() { axis };
    }

    private AxesCollection GetYAxis() => new()
    {
        new Axis()
        {
            MinValue = 0,
            Separator = new Separator()
            {
                Step = 1,
                Stroke = _coorporateDesignService.colGrid
            },
        }
    };
}