using System.Collections.Generic;

namespace LetterChart.Core.Models;

/// <summary>
/// Represents the generated chart content for rendering.
/// </summary>
public sealed class ChartDefinition
{
    public ChartDefinition(IReadOnlyList<ChartLineDefinition> lines, double screenDistanceMeters, double effectivePixelPitchMillimeters)
    {
        Lines = lines;
        ScreenDistanceMeters = screenDistanceMeters;
        EffectivePixelPitchMillimeters = effectivePixelPitchMillimeters;
    }

    public IReadOnlyList<ChartLineDefinition> Lines { get; }

    public double ScreenDistanceMeters { get; }

    public double EffectivePixelPitchMillimeters { get; }
}
