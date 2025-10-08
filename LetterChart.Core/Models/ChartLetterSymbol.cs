namespace LetterChart.Core.Models;

/// <summary>
/// Represents an individual optotype symbol and its rotation.
/// </summary>
public sealed class ChartLetterSymbol
{
    public ChartLetterSymbol(char symbol, double rotationDegrees)
    {
        Symbol = char.ToUpperInvariant(symbol);
        RotationDegrees = rotationDegrees;
    }

    public char Symbol { get; }

    public double RotationDegrees { get; }
}
