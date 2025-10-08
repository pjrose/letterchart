using System.Windows;
using System.Windows.Media;

namespace LetterChart;

public sealed class LetterChartViewOptions
{
    public static LetterChartViewOptions CreateDefault() => new();

    public Color FontColor { get; init; } = Colors.Black;

    public Color BackgroundColor { get; init; } = Colors.White;

    public Color LeaderLineColor { get; init; } = Color.FromRgb(96, 96, 96);

    public string FontFamilyName { get; init; } = "Sloan";

    public FontWeight FontWeight { get; init; } = FontWeights.SemiBold;

    public bool ShowLeaderLines { get; init; } = true;

    public double LetterSpacingFactor { get; init; } = 1.0;

    public double LineSpacingFactor { get; init; } = 1.0;

    public double LeaderFontSize { get; init; } = 14.0;

    public double LeaderLineLengthInches { get; init; } = 2.0;

    public double LeaderMarginInches { get; init; } = 0.5;
}
