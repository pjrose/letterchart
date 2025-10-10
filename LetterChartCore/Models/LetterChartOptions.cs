
using System.Collections.Generic;

namespace LetterChartCore.Models;

/// <summary>
/// Options describing how to create a letter chart.
/// </summary>
public sealed class LetterChartOptions
{
    public int NumberOfLines { get; set; } = 3;

    public double ScreenDistanceMeters { get; set; } = 4.0;

    /// <summary>
    /// Physical pixel pitch of the display in millimeters.
    /// </summary>
    public double PixelPitchMillimeters { get; set; } = 0.155;

    /// <summary>
    /// Operating system scaling percentage (100 == no scaling).
    /// </summary>
    public double ScalePercent { get; set; } = 100.0;

    /// <summary>
    /// Optional pre-defined letter strings for each line (one string per line).
    /// </summary>
    public IReadOnlyList<string>? ExplicitLineLetters { get; set; }

    /// <summary>
    /// Optional acuity targets for each line. If omitted defaults are used.
    /// </summary>
    public IReadOnlyList<SnellenAcuity>? AcuityOverrides { get; set; }

    /// <summary>
    /// Optional number of letters per line. If shorter than the number of lines the last value is repeated.
    /// </summary>
    public IReadOnlyList<int>? LettersPerLine { get; set; }

    public ChartVariant Variant { get; set; } = ChartVariant.Sloan;

    public ChartColor FontColor { get; set; } = ChartColor.FromRgb(0, 0, 0);

    public ChartColor BackgroundColor { get; set; } = ChartColor.FromRgb(255, 255, 255);

    public bool ShowLeaderLines { get; set; } = true;

    public double LeaderLineLengthInches { get; set; } = 2.0;

    public double LeaderFontSize { get; set; } = 18.0;

    public string FontFamilyName { get; set; } = "Sloan";

    public int? RandomSeed { get; set; }

    /// <summary>
    /// Letters that should be prioritised for random charts.
    /// </summary>
    public IReadOnlyList<char> PreferredLetters { get; set; } = new[] { 'C', 'D', 'E', 'F', 'L', 'N', 'O', 'P', 'T', 'Z' };

    /// <summary>
    /// Additional letters that can be used to avoid obvious repetition.
    /// </summary>
    public IReadOnlyList<char> SupplementalLetters { get; set; } = new[]
    {
        'B', 'G', 'H', 'K', 'R', 'S', 'U', 'V', 'Y'
    };
}
