using System;

namespace LetterChart.Core;

/// <summary>
/// Represents a Snellen visual acuity fraction (e.g. 20/20).
/// </summary>
public readonly struct SnellenFraction : IEquatable<SnellenFraction>
{
    public SnellenFraction(int numerator, int denominator)
    {
        if (numerator <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numerator), "Numerator must be positive.");
        }

        if (denominator <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(denominator), "Denominator must be positive.");
        }

        Numerator = numerator;
        Denominator = denominator;
    }

    public int Numerator { get; }

    public int Denominator { get; }

    public double Ratio => (double)Numerator / Denominator;

    public override string ToString() => $"{Numerator}/{Denominator}";

    public static bool TryParse(string value, out SnellenFraction fraction)
    {
        fraction = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var numerator))
        {
            return false;
        }

        if (!int.TryParse(parts[1], out var denominator))
        {
            return false;
        }

        if (numerator <= 0 || denominator <= 0)
        {
            return false;
        }

        fraction = new SnellenFraction(numerator, denominator);
        return true;
    }

    public static SnellenFraction Parse(string value)
    {
        if (!TryParse(value, out var fraction))
        {
            throw new FormatException($"'{value}' is not a valid Snellen fraction.");
        }

        return fraction;
    }

    public bool Equals(SnellenFraction other)
        => Numerator == other.Numerator && Denominator == other.Denominator;

    public override bool Equals(object? obj)
        => obj is SnellenFraction other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Numerator, Denominator);

    public static bool operator ==(SnellenFraction left, SnellenFraction right) => left.Equals(right);

    public static bool operator !=(SnellenFraction left, SnellenFraction right) => !left.Equals(right);
}
