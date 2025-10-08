using System.Windows;

namespace LetterChartApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ChartLauncher.ShowDemoChart();
    }
}
