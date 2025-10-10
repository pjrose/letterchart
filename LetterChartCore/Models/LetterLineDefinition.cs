
namespace LetterChartCore.Models;

/// <summary>
/// Represents a line of optotypes on the chart.
/// </summary>
public sealed class LetterLineDefinition
{
    public LetterLineDefinition(int lineNumber, SnellenAcuity acuity, IReadOnlyList<LetterDefinition> letters, double targetCapHeight, double bottomSpacing)
    {
        LineNumber = lineNumber;
        Acuity = acuity;
        Letters = letters;
        TargetCapHeight = targetCapHeight;
        BottomSpacing = bottomSpacing;
    }

    public int LineNumber { get; }

    public SnellenAcuity Acuity { get; }

    public IReadOnlyList<LetterDefinition> Letters { get; }

    /// <summary>
    /// Desired capital height of the glyphs in device independent pixels.
    /// </summary>
    public double TargetCapHeight { get; }

    /// <summary>
    /// Additional spacing to add below the line in device independent pixels.
    /// </summary>
    public double BottomSpacing { get; }
}
