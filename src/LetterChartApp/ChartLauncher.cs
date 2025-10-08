using System.Collections.Generic;
using System.Windows;
using LetterChart.Core;
using LetterChartApp.ViewModels;

namespace LetterChartApp;

public static class ChartLauncher
{
    // Sloan optotype font closely matches clinical Snellen charts.
    private const string DefaultFontFamily = "Sloan";

    public static void ShowDemoChart()
    {
        var options = new LetterChartOptions
        {
            ViewingDistanceMeters = 4.0,
            PixelPitchMillimeters = 0.155,
            ScaleFactor = 1.0,
            NumberOfLines = 3,
            AcuityTargets = new List<SnellenFraction>
            {
                new(20, 20),
                new(20, 30),
                new(20, 40),
            },
            LettersPerLine = new List<int> { 5, 5, 5 },
            ShowLeaderLines = true
        };

        var window = CreateChartWindow(options);
        Application.Current.MainWindow = window;
        window.Show();
    }

    public static ChartWindow CreateChartWindow(LetterChartOptions options, string fontFamily = DefaultFontFamily)
    {
        var builder = new LetterChartBuilder();
        var definition = builder.BuildChart(options);
        var viewModel = new ChartViewModel(definition, fontFamily);
        return new ChartWindow(viewModel);
    }

    public static ChartWindow CreateTumblingEChart(LetterChartOptions options, string fontFamily = DefaultFontFamily)
    {
        var builder = new LetterChartBuilder();
        var definition = builder.BuildTumblingChart(options, 'E');
        var viewModel = new ChartViewModel(definition, fontFamily);
        return new ChartWindow(viewModel);
    }

    public static ChartWindow CreateTumblingCChart(LetterChartOptions options, string fontFamily = DefaultFontFamily)
    {
        var builder = new LetterChartBuilder();
        var definition = builder.BuildTumblingChart(options, 'C');
        var viewModel = new ChartViewModel(definition, fontFamily);
        return new ChartWindow(viewModel);
    }

    public static void ShowTumblingEChart(LetterChartOptions options, string fontFamily = DefaultFontFamily)
    {
        var window = CreateTumblingEChart(options, fontFamily);
        Application.Current.MainWindow = window;
        window.Show();
    }

    public static void ShowTumblingCChart(LetterChartOptions options, string fontFamily = DefaultFontFamily)
    {
        var window = CreateTumblingCChart(options, fontFamily);
        Application.Current.MainWindow = window;
        window.Show();
    }
}
