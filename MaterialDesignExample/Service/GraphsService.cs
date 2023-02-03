using LiveCharts;
using LiveCharts.Wpf;
using SealWatch.Code.CutterLayer;
using SealWatch.Wpf.Service.Interfaces;
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
    private int _dayFilter = 7;
    private int _maxLocations = 7;

    public GraphsService(IDesignService coorporateDesignService)
    {
        _coorporateDesignService = coorporateDesignService;
    }

    public CartesianChart GetOrderedChart(List<AnalysedCutterDto> cutters, int dayFilter = 7)
    {
        _dayFilter = dayFilter;

        return new()
        {
            Series = GetFailuresCollection(cutters),
            AxisX = GetFailuresAxis(),
            AxisY = GetYAxis()
        };
    }

    public CartesianChart GetLocationChart(List<AnalysedCutterDto> cutters,  int dayFilter = 7)
    {
        _dayFilter = dayFilter;

        return new()
        {
            Series = GetLocationCollection(cutters),
            AxisX = GetLocationAxis(cutters),
            AxisY = GetYAxis()
        };
    }

    private SeriesCollection GetFailuresCollection(List<AnalysedCutterDto> cutters)
    {
        var cuttersOrdered = cutters.Where(x => x.SealOrdered).ToList();

        var seriesCollection = new SeriesCollection()
        {
            new LineSeries()
            {
                Title = "Nicht bestellt",
                LineSmoothness = 0.5,
                Fill = Brushes.Transparent,
                Stroke = Brushes.LightGray,
                Values = new ChartValues<int>()
            },
            new LineSeries()
            {
                Title = "Bestellt",
                LineSmoothness = 0.5,
                Fill = _coorporateDesignService.colGold60,
                Stroke = Brushes.Gold,
                Values = new ChartValues<int>()
            }
        };

        if (_dayFilter is 7)
        {
            for (int x = 0; x < 7; x++)
            {
                var cutterCount = cutters.Count(cutter => cutter.MillingStop.Date == DateTime.Now.AddDays(x).Date);
                seriesCollection[0].Values.Add(cutterCount);
            }

            for (int x = 0; x < 7; x++)
            {
                var cutterCount = cuttersOrdered.Count(cutter => cutter.MillingStop.Date == DateTime.Now.AddDays(x).Date);
                seriesCollection[1].Values.Add(cutterCount);
            }
        }
        else if (_dayFilter is 31)
        {
            var weekOne = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            var weekTwo = new List<int>() { 8, 9, 10, 11, 12, 13, 14 };
            var weekThree = new List<int>() { 15, 16, 17, 18, 19, 20, 21 };
            var weekFour = new List<int>() { 22, 23, 24, 25, 26, 27, 28 };

            var cutterThisYearMonth = cutters.Where(cutter => cutter.MillingStop.Year == DateTime.Now.Year && cutter.MillingStop.Month == DateTime.Now.Month).ToList();

            if (cutterThisYearMonth is not null)
            {
                seriesCollection[0].Values = new ChartValues<int>()
                {
                    cutterThisYearMonth.Count(x => weekOne.Contains(x.MillingStop.Day)),
                    cutterThisYearMonth.Count(x => weekTwo.Contains(x.MillingStop.Day)),
                    cutterThisYearMonth.Count(x => weekThree.Contains(x.MillingStop.Day)),
                    cutterThisYearMonth.Count(x => weekFour.Contains(x.MillingStop.Day)),
                };

                seriesCollection[1].Values = new ChartValues<int>()
                {
                    cutterThisYearMonth.Count(x => x.SealOrdered && weekOne.Contains(x.MillingStop.Day)),
                    cutterThisYearMonth.Count(x => x.SealOrdered && weekTwo.Contains(x.MillingStop.Day)),
                    cutterThisYearMonth.Count(x => x.SealOrdered && weekThree.Contains(x.MillingStop.Day)),
                    cutterThisYearMonth.Count(x => x.SealOrdered && weekFour.Contains(x.MillingStop.Day)),
                };
            }
        }
        else if (_dayFilter is 360)
        {
            for (int x = 0; x < 12; x++)
            {
                seriesCollection[0].Values.Add(cutters.Count(item => item.MillingStop.Month == DateTime.Now.AddMonths(x).Month
                                                                  && item.MillingStop.Year == DateTime.Now.AddMonths(x).Year));
            }

            for (int x = 0; x < 12; x++)
            {
                seriesCollection[1].Values.Add(cutters.Count(item => item.SealOrdered
                                                                  && item.MillingStop.Month == DateTime.Now.AddMonths(x).Month
                                                                  && item.MillingStop.Year == DateTime.Now.AddMonths(x).Year));
            }
        }

        return seriesCollection;
    }
    private AxesCollection GetFailuresAxis()
    {
        var axis = new Axis()
        {
            Labels = new List<string>(),
            Foreground = Brushes.Black,
            Separator = new Separator()
            {
                Step = 1,
                Stroke = _coorporateDesignService.colGrid
            }
        };

        if (_dayFilter is 7)
        {
            for (int x = 0; x < 7; x++)
            {
                axis.Labels.Add(DateTime.Now.AddDays(x).ToString("dd MMM"));
            }
        }
        else if (_dayFilter is 31)
        {
            for (int x = 1; x < 5; x++)
            {
                axis.Labels.Add($"Woche {x}");
            }
        }
        else if (_dayFilter is 360)
        {
            for (int x = 0; x < 12; x++)
            {
                axis.Labels.Add(DateTime.Now.AddMonths(x).ToString("MMM"));
            }
        }

        return new AxesCollection() { axis };
    }

    private SeriesCollection GetLocationCollection(List<AnalysedCutterDto> cutters)
    {
        var columnSeries = new ColumnSeries()
        {
            Title = "Standort",
            DataLabels = true,
            Values = new ChartValues<int>(),
            Fill = _coorporateDesignService.colGold160,
        };

        var locations = cutters.Select(x => x.Location).Distinct().ToList();

        if (locations.Count > _maxLocations)
            locations = locations.Take(_maxLocations).ToList();

        foreach (var location in locations)
        {
            var cuttersAtLocation = cutters.Count(x => x.Location == location);
            columnSeries.Values.Add(cuttersAtLocation);
        }

        return new SeriesCollection { columnSeries };
    }
    private AxesCollection GetLocationAxis(List<AnalysedCutterDto> cutters)
    {
        var axis = new Axis()
        {
            Labels = cutters.Select(x => x.Location).Distinct().ToArray(),
            Foreground = Brushes.Black,
            Separator = new()
            {
                Step = 1,
                Stroke = _coorporateDesignService.colGrid
            }
        };

        return new AxesCollection() { axis };
    }

    private AxesCollection GetYAxis()
    {
        return new()
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
}