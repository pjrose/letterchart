using System.Windows;
using LetterChart.Core;
using LetterChart.Core.Models;

namespace LetterChart;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ChartLauncher.ShowDemoChart();
    }
}
