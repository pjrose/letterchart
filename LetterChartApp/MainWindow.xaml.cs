
using System.Windows;

namespace LetterChartApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnLaunchSloan(object sender, RoutedEventArgs e)
    {
        LetterChartLauncher.ShowClinicalTestChart();
    }

    private void OnLaunchTumblingE(object sender, RoutedEventArgs e)
    {
        LetterChartLauncher.ShowClinicalTestChart(variant: LetterChartCore.Models.ChartVariant.TumblingE);
    }

    private void OnLaunchTumblingC(object sender, RoutedEventArgs e)
    {
        LetterChartLauncher.ShowClinicalTestChart(variant: LetterChartCore.Models.ChartVariant.TumblingC);
    }
}
