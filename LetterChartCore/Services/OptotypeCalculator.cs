
using LetterChartCore.Models;

namespace LetterChartCore.Services;

/// <summary>
/// Provides calculation helpers for chart rendering.
/// </summary>
public static class OptotypeCalculator
{
    private const double ArcMinutesPerDegree = 60.0;
    private const double DegreesToRadians = Math.PI / 180.0;

    /// <summary>
    /// Calculates the physical letter height in meters for a given Snellen acuity and distance.
    /// </summary>
    public static double CalculateLetterHeightMeters(double distanceMeters, SnellenAcuity acuity)
    {
        if (distanceMeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceMeters));
        }

        var minutesOfArc = 5.0 * acuity.Denominator / acuity.Numerator;
        var angleRadians = (minutesOfArc / ArcMinutesPerDegree) * DegreesToRadians;
        return 2 * distanceMeters * Math.Tan(angleRadians / 2.0);
    }

    /// <summary>
    /// Converts a physical letter height (meters) into WPF device independent pixels.
    /// </summary>
    public static double ConvertMetersToDeviceIndependentPixels(double heightMeters, double pixelPitchMillimeters, double scalePercent)
    {
        if (heightMeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(heightMeters));
        }

        if (pixelPitchMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pixelPitchMillimeters));
        }

        var pixelPitchMeters = pixelPitchMillimeters / 1000.0;
        var scaleFactor = Math.Max(scalePercent, 1e-3) / 100.0;
        var effectivePixelPitch = pixelPitchMeters / scaleFactor;
        return heightMeters / effectivePixelPitch;
    }

    /// <summary>
    /// Determines the default letter counts for each line.
    /// </summary>
    public static IReadOnlyList<int> GetLetterCounts(int numberOfLines, IReadOnlyList<int>? overrides)
    {
        var defaults = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var result = new int[numberOfLines];
        for (var i = 0; i < numberOfLines; i++)
        {
            if (overrides != null && i < overrides.Count)
            {
                result[i] = Math.Max(1, overrides[i]);
            }
            else
            {
                result[i] = defaults[Math.Min(i, defaults.Length - 1)];
            }
        }

        return result;
    }

    public static IReadOnlyList<SnellenAcuity> GetAcuities(int numberOfLines, IReadOnlyList<SnellenAcuity>? overrides)
    {
        var defaults = new[]
        {
            new SnellenAcuity(20, 10),
            new SnellenAcuity(20, 15),
            new SnellenAcuity(20, 20),
            new SnellenAcuity(20, 25),
            new SnellenAcuity(20, 30),
            new SnellenAcuity(20, 40),
            new SnellenAcuity(20, 50),
            new SnellenAcuity(20, 60),
            new SnellenAcuity(20, 80),
            new SnellenAcuity(20, 100),
            new SnellenAcuity(20, 200)
        };

        var result = new SnellenAcuity[numberOfLines];
        for (var i = 0; i < numberOfLines; i++)
        {
            if (overrides != null && i < overrides.Count)
            {
                result[i] = overrides[i];
            }
            else if (numberOfLines == 3)
            {
                // Honour the requested defaults for the common 3-line quick chart.
                var defaultsThree = new[]
                {
                    new SnellenAcuity(20, 20),
                    new SnellenAcuity(20, 30),
                    new SnellenAcuity(20, 40)
                };
                result[i] = defaultsThree[i];
            }
            else
            {
                result[i] = defaults[Math.Min(i, defaults.Length - 1)];
            }
        }

        return result;
    }
}
