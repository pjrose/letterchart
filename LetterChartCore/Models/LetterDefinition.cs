
namespace LetterChartCore.Models;

/// <summary>
/// Represents a single optotype glyph to render.
/// </summary>
public sealed class LetterDefinition
{
    public LetterDefinition(char glyph, double targetCapHeight, double letterSpacing, double rotationDegrees = 0)
    {
        Glyph = char.ToUpperInvariant(glyph);
        TargetCapHeight = targetCapHeight;
        LetterSpacing = letterSpacing;
        RotationDegrees = rotationDegrees;
    }

    public char Glyph { get; }

    /// <summary>
    /// Desired capital height of the glyph in device independent pixels.
    /// </summary>
    public double TargetCapHeight { get; }

    /// <summary>
    /// Horizontal spacing to apply after this glyph in device independent pixels.
    /// </summary>
    public double LetterSpacing { get; }

    /// <summary>
    /// Rotation to apply for tumbling optotypes.
    /// </summary>
    public double RotationDegrees { get; }
}
