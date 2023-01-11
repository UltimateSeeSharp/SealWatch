using LiveCharts;
using LiveCharts.Wpf;
using SealWatch.Data.Model;
using SealwatchTest.Components;
using SealwatchTest.Extentions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SealwatchTest.ViewModel;

public class MainWindowViewModel : BaseViewModel
{
    public MainWindowViewModel()
    {

    }

    private LazyProperty<CartesianChart> _chart;
    public LazyProperty<CartesianChart> Chart =>
        _chart ??= new LazyProperty<CartesianChart>(GetChart);

    private async Task<CartesianChart> GetChart(CancellationToken cancellationToken)
    {
        await Task.Delay(2000);

        var chart = new CartesianChart();
        chart.Series = new LiveCharts.SeriesCollection()
        {
            new LineSeries()
            {
                Title = "Tst",
                Values = new ChartValues<int>() {1,2,3,4,5}
            }
        };

        return chart;
    }
}

public class Person
{
    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}