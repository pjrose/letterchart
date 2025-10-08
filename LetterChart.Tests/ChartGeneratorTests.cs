using System;
using System.Linq;
using LetterChart.Core;
using LetterChart.Core.Models;
using Xunit;

namespace LetterChart.Tests;

public class ChartGeneratorTests
{
    [Fact]
    public void CalculatesExpectedLetterHeightFor20_20AtFourMeters()
    {
        var height = MeasurementUtilities.CalculateOptotypeHeightMillimetres(4.0, 20, 20);
        Assert.InRange(height, 5.7, 5.9);
    }

    [Fact]
    public void CalculatesExpectedLetterHeightFor20_40AtFourMeters()
    {
        var height = MeasurementUtilities.CalculateOptotypeHeightMillimetres(4.0, 20, 40);
        Assert.InRange(height, 11.5, 11.8);
    }

    [Fact]
    public void GeneratesLinesMatchingRequestedCount()
    {
        var parameters = new ChartParameters
        {
            NumberOfLines = 4,
            ScreenDistanceMeters = 4.0,
            PixelPitchMillimeters = 0.155,
            RandomSeed = 42
        };

        var chart = ChartGenerator.GenerateChart(parameters);
        Assert.Equal(4, chart.Lines.Count);
        Assert.Equal(new[] { 1, 2, 3, 4 }, chart.Lines.Select(l => l.Letters.Count));
    }

    [Fact]
    public void GeneratesDeterministicSloanLettersWithSeed()
    {
        var parameters = new ChartParameters
        {
            NumberOfLines = 3,
            ScreenDistanceMeters = 4.0,
            PixelPitchMillimeters = 0.155,
            RandomSeed = 123
        };

        var first = ChartGenerator.GenerateChart(parameters);
        var second = ChartGenerator.GenerateChart(parameters);
        Assert.Equal(first.Lines.Select(l => string.Concat(l.Letters.Select(s => s.Symbol))),
                     second.Lines.Select(l => string.Concat(l.Letters.Select(s => s.Symbol))));
    }

    [Fact]
    public void GeneratesTumblingEOrientations()
    {
        var parameters = new ChartParameters
        {
            NumberOfLines = 2,
            ScreenDistanceMeters = 4.0,
            PixelPitchMillimeters = 0.155,
            SymbolMode = ChartSymbolMode.TumblingE,
            RandomSeed = 7
        };

        var chart = ChartGenerator.GenerateChart(parameters);
        var allowed = new double[] { 0, 90, 180, 270 };
        Assert.All(chart.Lines.SelectMany(l => l.Letters), symbol =>
        {
            Assert.Equal('E', symbol.Symbol);
            Assert.Contains(symbol.RotationDegrees, allowed);
        });
    }

    [Fact]
    public void GeneratesLandoltCOrientations()
    {
        var allowed = new double[] { 0, 45, 90, 135, 180, 225, 270, 315 };
        var parameters = new ChartParameters
        {
            NumberOfLines = 2,
            ScreenDistanceMeters = 4.0,
            PixelPitchMillimeters = 0.155,
            SymbolMode = ChartSymbolMode.LandoltC,
            RandomSeed = 11
        };

        var chart = ChartGenerator.GenerateChart(parameters);
        Assert.All(chart.Lines.SelectMany(l => l.Letters), symbol =>
        {
            Assert.Equal('C', symbol.Symbol);
            Assert.Contains(symbol.RotationDegrees, allowed);
        });
    }

    [Fact]
    public void UsesProvidedLetterSequences()
    {
        var parameters = new ChartParameters
        {
            NumberOfLines = 2,
            ScreenDistanceMeters = 4.0,
            PixelPitchMillimeters = 0.155,
            LetterSequences = new[] { "ABC", "DEF" }
        };

        var chart = ChartGenerator.GenerateChart(parameters);
        Assert.Equal("ABC", string.Concat(chart.Lines[0].Letters.Select(l => l.Symbol)));
        Assert.Equal("DEF", string.Concat(chart.Lines[1].Letters.Select(l => l.Symbol)));
    }
}
