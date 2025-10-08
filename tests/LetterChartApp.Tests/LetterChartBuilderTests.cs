using System.Linq;
using LetterChart.Core;
using Xunit;

namespace LetterChartApp.Tests;

public class LetterChartBuilderTests
{
    [Fact]
    public void GeneratesLinesWithUppercaseLetters()
    {
        var options = new LetterChartOptions
        {
            ViewingDistanceMeters = 4,
            PixelPitchMillimeters = 0.155,
            NumberOfLines = 2,
            AcuityTargets = new[] { new SnellenFraction(20, 20), new SnellenFraction(20, 30) },
            LettersPerLine = new[] { 5, 5 },
            LetterSequences = new[] { "abcde", "fghij" }
        };

        var builder = new LetterChartBuilder();
        var chart = builder.BuildChart(options);

        Assert.All(chart.Lines, line => Assert.True(line.Letters.All(char.IsUpper)));
    }

    [Fact]
    public void TumblingChartProvidesRotations()
    {
        var options = new LetterChartOptions
        {
            ViewingDistanceMeters = 4,
            PixelPitchMillimeters = 0.155,
            NumberOfLines = 1,
            AcuityTargets = new[] { new SnellenFraction(20, 20) },
            LettersPerLine = new[] { 4 }
        };

        var builder = new LetterChartBuilder();
        var chart = builder.BuildTumblingChart(options, 'E');

        var line = Assert.Single(chart.Lines);
        Assert.All(line.Letters, c => Assert.Equal('E', c));
        Assert.All(line.Rotations, angle => Assert.Contains(angle, new[] { 0d, 90d, 180d, 270d }));
    }
}
