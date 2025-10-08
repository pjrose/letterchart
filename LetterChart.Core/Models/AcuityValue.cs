using System;

namespace LetterChart.Core.Models;

/// <summary>
/// Represents a Snellen-style acuity fraction such as 20/20.
/// </summary>
public readonly record struct AcuityValue(int Numerator, int Denominator)
{
    public string Display => $"{Numerator}/{Denominator}";

    public double Ratio => Numerator <= 0 ? throw new InvalidOperationException("Numerator must be positive.") : (double)Denominator / Numerator;

    public static AcuityValue Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Acuity string cannot be null or empty.", nameof(input));
        }

        var parts = input.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || !int.TryParse(parts[0], out var numerator) || !int.TryParse(parts[1], out var denominator))
        {
            throw new FormatException($"Invalid acuity fraction '{input}'.");
        }

        return new AcuityValue(numerator, denominator);
    }
}
