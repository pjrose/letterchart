using LetterChart.Core;
using Xunit;

namespace LetterChartApp.Tests;

public class SnellenMathTests
{
    [Theory]
    [InlineData(4.0, 20, 20, 5.82, 0.05)]
    [InlineData(6.0, 20, 40, 17.45, 0.1)]
    public void CalculatesExpectedLetterHeight(double distance, int numerator, int denominator, double expectedMm, double tolerance)
    {
        var acuity = new SnellenFraction(numerator, denominator);
        var actual = SnellenMath.GetLetterHeightMillimeters(distance, acuity);
        Assert.InRange(actual, expectedMm - tolerance, expectedMm + tolerance);
    }

    [Fact]
    public void ConvertsToPixelsUsingPixelPitch()
    {
        var acuity = new SnellenFraction(20, 20);
        var pixels = SnellenMath.GetLetterHeightPixels(4.0, 0.155, 1.0, acuity);
        var expectedMm = SnellenMath.GetLetterHeightMillimeters(4.0, acuity);
        var expectedPixels = expectedMm / 0.155;
        Assert.InRange(pixels, expectedPixels - 0.01, expectedPixels + 0.01);
    }
}
