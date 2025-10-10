namespace LetterChartCore.Models;

/// <summary>
/// Lightweight ARGB color representation used to decouple the core logic from UI frameworks.
/// </summary>
public readonly record struct ChartColor(byte A, byte R, byte G, byte B)
{
    public static ChartColor FromRgb(byte r, byte g, byte b) => new(255, r, g, b);
}
