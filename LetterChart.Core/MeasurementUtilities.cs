using System;

namespace LetterChart.Core;

/// <summary>
/// Provides conversions used during chart generation.
/// </summary>
public static class MeasurementUtilities
{
    private const double MinutesPerDegree = 60.0;
    private const double DegreesToRadians = Math.PI / 180.0;

    /// <summary>
    /// Calculates the physical optotype height in millimetres for the supplied acuity value.
    /// </summary>
    public static double CalculateOptotypeHeightMillimetres(double viewingDistanceMeters, int acuityNumerator, int acuityDenominator)
    {
        if (viewingDistanceMeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewingDistanceMeters));
        }

        if (acuityNumerator <= 0 || acuityDenominator <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(acuityNumerator), "Acuity values must be positive.");
        }

        var baseArcMinutes = 5.0 * acuityDenominator / acuityNumerator;
        var angleRadians = (baseArcMinutes / MinutesPerDegree) * DegreesToRadians;
        var heightMeters = 2.0 * viewingDistanceMeters * Math.Tan(angleRadians / 2.0);
        return heightMeters * 1000.0; // convert to mm
    }

    public static double MillimetresToDeviceIndependentPixels(double millimetres)
    {
        const double millimetresPerInch = 25.4;
        const double dipPerInch = 96.0;
        return millimetres / millimetresPerInch * dipPerInch;
    }

    public static double InchesToDeviceIndependentPixels(double inches)
        => inches * 96.0;
}
