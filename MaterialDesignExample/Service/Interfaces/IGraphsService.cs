using LiveCharts.Wpf;
using SealWatch.Code.CutterLayer;
using SealWatch.Code.Enums;
using System.Collections.Generic;

namespace SealWatch.Wpf.Service.Interfaces;

public interface IGraphsService
{
    CartesianChart GetCutterLocationChart(List<AnalysedCutterDto> cutters);
    CartesianChart GetOrderedSealsChart(List<AnalysedCutterDto> cutters, Timeframe timeframe = Timeframe.Week);
}