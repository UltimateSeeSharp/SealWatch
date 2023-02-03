using LiveCharts.Wpf;
using SealWatch.Code.CutterLayer;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SealWatch.Wpf.Service.Interfaces;

public interface IGraphsService
{
    CartesianChart GetOrderedChart(List<AnalysedCutterDto> cutters, int dayFilter = 7);
    CartesianChart GetLocationChart(List<AnalysedCutterDto> cutters, int dayFilter = 7);
}