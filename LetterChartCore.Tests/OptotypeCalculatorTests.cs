
using System.Linq;
using LetterChartCore.Models;
using LetterChartCore.Services;

namespace LetterChartCore.Tests;

public class OptotypeCalculatorTests
{
    [Fact]
    public void CalculateLetterHeightMeters_MatchesExpected20_20()
    {
        var acuity = new SnellenAcuity(20, 20);
        var height = OptotypeCalculator.CalculateLetterHeightMeters(4.0, acuity);
        Assert.InRange(height, 0.0057, 0.0059);
    }

    [Fact]
    public void ConvertMetersToDeviceIndependentPixels_UsesScalePercent()
    {
        var heightMeters = 0.0058;
        var pixels = OptotypeCalculator.ConvertMetersToDeviceIndependentPixels(heightMeters, 0.155, 100);
        Assert.InRange(pixels, 37.0, 38.5);

        var scaled = OptotypeCalculator.ConvertMetersToDeviceIndependentPixels(heightMeters, 0.155, 150);
        Assert.True(scaled > pixels);
    }

    [Fact]
    public void ChartLayoutBuilder_BuildsLinesWithExpectedCounts()
    {
        var options = new LetterChartOptions
        {
            NumberOfLines = 3,
            ScreenDistanceMeters = 4.0,
            PixelPitchMillimeters = 0.155,
            ScalePercent = 100,
            Variant = ChartVariant.Sloan,
            RandomSeed = 123
        };

        var builder = new ChartLayoutBuilder();
        var lines = builder.Build(options);

        Assert.Equal(3, lines.Count);
        Assert.Equal(new[] { 1, 2, 3 }, lines.Select(l => l.Letters.Count));
        Assert.True(lines[0].TargetCapHeight < lines[1].TargetCapHeight);
    }

    [Fact]
    public void ChartLayoutBuilder_CreatesTumblingERotations()
    {
        var options = new LetterChartOptions
        {
            NumberOfLines = 1,
            PixelPitchMillimeters = 0.155,
            ScreenDistanceMeters = 4.0,
            Variant = ChartVariant.TumblingE,
            LettersPerLine = new[] { 12 },
            RandomSeed = 42
        };

        var builder = new ChartLayoutBuilder();
        var lines = builder.Build(options);
        var rotations = lines[0].Letters.Select(l => l.RotationDegrees).Distinct().ToList();
        Assert.True(rotations.Count >= 2);
        var allowed = new[] { 0d, 90d, 180d, 270d };
        Assert.All(rotations, angle => Assert.Contains(angle, allowed));
    }
}
