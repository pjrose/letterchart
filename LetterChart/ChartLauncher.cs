using System.Windows;
using LetterChart.Core;
using LetterChart.Core.Models;

namespace LetterChart;

public static class ChartLauncher
{
    public static ChartParameters CreateDefaultParameters() => new()
    {
        NumberOfLines = 3,
        ScreenDistanceMeters = 4.0,
        PixelPitchMillimeters = 0.155,
        ScalePercent = 100.0,
        AcuityTargets = new[]
        {
            new AcuityValue(20, 20),
            new AcuityValue(20, 30),
            new AcuityValue(20, 40)
        }
    };

    public static LetterChartViewOptions CreateDefaultViewOptions() => LetterChartViewOptions.CreateDefault();

    public static LetterChartWindow ShowDemoChart()
    {
        var window = CreateChartWindow(CreateDefaultParameters(), CreateDefaultViewOptions());
        window.Show();
        if (Application.Current is { } app)
        {
            app.MainWindow = window;
        }

        return window;
    }

    public static LetterChartWindow ShowTumblingEChart(double scalePercent = 100.0)
    {
        var parameters = CreateDefaultParameters() with
        {
            SymbolMode = ChartSymbolMode.TumblingE,
            ScalePercent = scalePercent
        };

        var window = CreateChartWindow(parameters, CreateDefaultViewOptions());
        window.Show();
        if (Application.Current is { } app)
        {
            app.MainWindow = window;
        }

        return window;
    }

    public static LetterChartWindow ShowLandoltCChart(double scalePercent = 100.0)
    {
        var parameters = CreateDefaultParameters() with
        {
            SymbolMode = ChartSymbolMode.LandoltC,
            ScalePercent = scalePercent
        };

        var window = CreateChartWindow(parameters, CreateDefaultViewOptions());
        window.Show();
        if (Application.Current is { } app)
        {
            app.MainWindow = window;
        }

        return window;
    }

    public static LetterChartWindow CreateChartWindow(ChartParameters parameters, LetterChartViewOptions? viewOptions = null)
    {
        return new LetterChartWindow(parameters, viewOptions);
    }
}
