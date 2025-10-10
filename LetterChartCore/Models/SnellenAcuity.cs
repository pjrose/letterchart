using System.Globalization;

namespace LetterChartCore.Models;

/// <summary>
/// Represents a Snellen visual acuity value.
/// </summary>
public readonly record struct SnellenAcuity(int Numerator, int Denominator)
{
    public override string ToString() => $"{Numerator}/{Denominator}";

    public static bool TryParse(string value, out SnellenAcuity acuity)
    {
        acuity = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        if (int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var numerator) &&
            int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var denominator) &&
            numerator > 0 && denominator > 0)
        {
            acuity = new SnellenAcuity(numerator, denominator);
            return true;
        }

        return false;
    }
}
