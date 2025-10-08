using System;

namespace LetterChart.Core;

/// <summary>
/// Provides utilities for computing Snellen letter dimensions and spacing.
/// </summary>
public static class SnellenMath
{
    private const double BaseArcMinutes = 5.0;

    public static double GetLetterHeightMillimeters(double viewingDistanceMeters, SnellenFraction acuity)
    {
        if (viewingDistanceMeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewingDistanceMeters), "Viewing distance must be positive.");
        }

        var arcMinutes = BaseArcMinutes * acuity.Denominator / acuity.Numerator;
        var radians = DegreesToRadians(arcMinutes / 60.0);
        var halfAngle = radians / 2.0;
        var distanceMillimeters = viewingDistanceMeters * 1000.0;
        return 2.0 * distanceMillimeters * Math.Tan(halfAngle);
    }

    public static double GetLetterHeightPixels(double viewingDistanceMeters, double pixelPitchMillimeters, double scaleFactor, SnellenFraction acuity)
    {
        if (pixelPitchMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pixelPitchMillimeters), "Pixel pitch must be positive.");
        }

        var heightMm = GetLetterHeightMillimeters(viewingDistanceMeters, acuity) * scaleFactor;
        return heightMm / pixelPitchMillimeters;
    }

    public static double GetLetterSpacingPixels(double letterHeightPixels)
        => letterHeightPixels; // ISO 8596 recommends spacing equal to letter width.

    public static double GetLineSpacingPixels(double letterHeightPixels)
        => letterHeightPixels; // Maintain one letter height between rows.

    public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}
