using System.Collections.Generic;

namespace LetterChart.Core.Models;

/// <summary>
/// Describes a single line of optotypes in the chart.
/// </summary>
public sealed class ChartLineDefinition
{
    public ChartLineDefinition(AcuityValue acuity, double letterHeightMillimeters, IReadOnlyList<ChartLetterSymbol> letters)
    {
        Acuity = acuity;
        LetterHeightMillimeters = letterHeightMillimeters;
        Letters = letters;
    }

    public AcuityValue Acuity { get; }

    public double LetterHeightMillimeters { get; }

    public IReadOnlyList<ChartLetterSymbol> Letters { get; }
}
