using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LetterChart.Core.Models;

namespace LetterChart.Core;

public static class ChartGenerator
{
    private static readonly char[] SloanPreferredLetters = { 'C', 'D', 'E', 'F', 'L', 'N', 'O', 'P', 'T', 'Z' };
    private static readonly char[] AdditionalLetters = "ABGHJKLMRSUVWXY".ToCharArray();
    private static readonly double[] TumblingERotations = { 0, 90, 180, 270 };
    private static readonly double[] LandoltCRotations = { 0, 45, 90, 135, 180, 225, 270, 315 };

    public static ChartDefinition GenerateChart(ChartParameters parameters)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (parameters.ScreenDistanceMeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parameters.ScreenDistanceMeters));
        }

        if (parameters.PixelPitchMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parameters.PixelPitchMillimeters));
        }

        if (parameters.ScalePercent <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parameters.ScalePercent));
        }

        var acuities = parameters.ResolveAcuities();
        var letterCounts = parameters.ResolveLetterCounts();
        var rng = parameters.RandomSeed.HasValue ? new Random(parameters.RandomSeed.Value) : new Random();

        var lines = new List<ChartLineDefinition>(acuities.Count);
        for (var i = 0; i < acuities.Count; i++)
        {
            var acuity = acuities[i];
            var letterHeightMm = MeasurementUtilities.CalculateOptotypeHeightMillimetres(parameters.ScreenDistanceMeters, acuity.Numerator, acuity.Denominator);
            var letters = BuildLettersForLine(parameters, letterCounts[i], rng, i);
            lines.Add(new ChartLineDefinition(acuity, letterHeightMm, letters));
        }

        return new ChartDefinition(lines, parameters.ScreenDistanceMeters, parameters.EffectivePixelPitch);
    }

    private static IReadOnlyList<ChartLetterSymbol> BuildLettersForLine(ChartParameters parameters, int letterCount, Random rng, int lineIndex)
    {
        if (parameters.LetterSequences is { Count: > 0 } && lineIndex < parameters.LetterSequences.Count)
        {
            var sequence = parameters.LetterSequences[lineIndex];
            return sequence.Select(c => new ChartLetterSymbol(char.ToUpperInvariant(c), DetermineRotation(parameters.SymbolMode, rng))).ToArray();
        }

        return parameters.SymbolMode switch
        {
            ChartSymbolMode.SloanLetters => GenerateSloanLetters(letterCount, rng, parameters.RandomiseLetters),
            ChartSymbolMode.TumblingE => GenerateRepeatedLetter('E', letterCount, rng, TumblingERotations),
            ChartSymbolMode.LandoltC => GenerateRepeatedLetter('C', letterCount, rng, LandoltCRotations),
            _ => throw new NotSupportedException($"Unsupported symbol mode {parameters.SymbolMode}.")
        };
    }

    private static IReadOnlyList<ChartLetterSymbol> GenerateSloanLetters(int letterCount, Random rng, bool randomise)
    {
        var letters = new List<ChartLetterSymbol>(letterCount);

        if (!randomise)
        {
            for (var i = 0; i < letterCount; i++)
            {
                var symbol = SloanPreferredLetters[i % SloanPreferredLetters.Length];
                letters.Add(new ChartLetterSymbol(symbol, 0));
            }

            return letters;
        }

        var preferredWeight = SloanPreferredLetters.Length * 2;
        Span<char> pool = stackalloc char[preferredWeight + AdditionalLetters.Length];
        SloanPreferredLetters.CopyTo(pool);
        SloanPreferredLetters.CopyTo(pool[SloanPreferredLetters.Length..preferredWeight]);
        AdditionalLetters.CopyTo(pool[preferredWeight..]);

        for (var i = 0; i < letterCount; i++)
        {
            var index = rng.Next(pool.Length);
            letters.Add(new ChartLetterSymbol(pool[index], 0));
        }

        return letters;
    }

    private static IReadOnlyList<ChartLetterSymbol> GenerateRepeatedLetter(char symbol, int letterCount, Random rng, IReadOnlyList<double> rotations)
    {
        var letters = new List<ChartLetterSymbol>(letterCount);
        for (var i = 0; i < letterCount; i++)
        {
            var rotation = rotations[rng.Next(rotations.Count)];
            letters.Add(new ChartLetterSymbol(symbol, rotation));
        }

        return letters;
    }

    private static double DetermineRotation(ChartSymbolMode mode, Random rng)
    {
        return mode switch
        {
            ChartSymbolMode.TumblingE => TumblingERotations[rng.Next(TumblingERotations.Length)],
            ChartSymbolMode.LandoltC => LandoltCRotations[rng.Next(LandoltCRotations.Length)],
            _ => 0
        };
    }
}
