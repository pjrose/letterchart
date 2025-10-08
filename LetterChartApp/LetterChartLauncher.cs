
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using LetterChartApp.Views;
using LetterChartCore.Models;

namespace LetterChartApp;

public static class LetterChartLauncher
{
    public static LetterChartWindow ShowChart(LetterChartOptions options)
    {
        var window = new LetterChartWindow();
        window.InitializeChart(options);
        if (Application.Current != null)
        {
            window.Owner = Application.Current.MainWindow;
        }

        window.Show();
        window.Activate();
        return window;
    }

    public static LetterChartWindow ShowClinicalTestChart(
        ChartVariant variant = ChartVariant.Sloan,
        int numberOfLines = 3,
        double pixelPitchMillimeters = 0.155,
        double screenDistanceMeters = 4.0,
        double scalePercent = 100.0,
        IEnumerable<string>? explicitLineLetters = null,
        IEnumerable<string>? acuityTargets = null,
        IEnumerable<int>? lettersPerLine = null,
        Color? fontColor = null,
        Color? backgroundColor = null,
        bool showLeaderLines = true)
    {
        var options = new LetterChartOptions
        {
            Variant = variant,
            NumberOfLines = numberOfLines,
            PixelPitchMillimeters = pixelPitchMillimeters,
            ScreenDistanceMeters = screenDistanceMeters,
            ScalePercent = scalePercent,
            ExplicitLineLetters = explicitLineLetters?.Select(s => s.ToUpperInvariant()).ToList(),
            LettersPerLine = lettersPerLine?.ToList(),
            AcuityOverrides = acuityTargets?.Select(ParseAcuity).Where(a => a != null).Select(a => a!.Value).ToList(),
            FontColor = fontColor.HasValue ? ToChartColor(fontColor.Value) : ChartColor.FromRgb(0, 0, 0),
            BackgroundColor = backgroundColor.HasValue ? ToChartColor(backgroundColor.Value) : ChartColor.FromRgb(255, 255, 255),
            ShowLeaderLines = showLeaderLines
        };

        return ShowChart(options);
    }

    private static SnellenAcuity? ParseAcuity(string value)
    {
        return SnellenAcuity.TryParse(value, out var acuity) ? acuity : null;
    }

    private static ChartColor ToChartColor(Color color)
    {
        return new ChartColor(color.A, color.R, color.G, color.B);
    }
}
