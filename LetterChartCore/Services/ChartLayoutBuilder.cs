
using System.Collections.Generic;
using System.Linq;
using LetterChartCore.Models;

namespace LetterChartCore.Services;

/// <summary>
/// Builds letter line definitions based on chart options.
/// </summary>
public sealed class ChartLayoutBuilder
{
    public IReadOnlyList<LetterLineDefinition> Build(LetterChartOptions options)
    {
        if (options.NumberOfLines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.NumberOfLines));
        }

        var generator = new LetterSequenceGenerator(options.RandomSeed);
        var acuities = OptotypeCalculator.GetAcuities(options.NumberOfLines, options.AcuityOverrides);
        var letterCounts = OptotypeCalculator.GetLetterCounts(options.NumberOfLines, options.LettersPerLine);
        var lineLetters = ResolveLineLetters(options, letterCounts, generator);
        var orientationRandom = options.RandomSeed.HasValue ? new Random(options.RandomSeed.Value) : new Random();

        var lines = new List<LetterLineDefinition>(options.NumberOfLines);
        double? nextLineCapHeight = null;
        for (var i = options.NumberOfLines - 1; i >= 0; i--)
        {
            var acuity = acuities[i];
            var heightMeters = OptotypeCalculator.CalculateLetterHeightMeters(options.ScreenDistanceMeters, acuity);
            var capHeightDips = OptotypeCalculator.ConvertMetersToDeviceIndependentPixels(heightMeters, options.PixelPitchMillimeters, options.ScalePercent);
            var letters = CreateLetters(lineLetters[i], letterCounts[i], capHeightDips, options, orientationRandom);
            var bottomSpacing = nextLineCapHeight ?? 0;
            lines.Insert(0, new LetterLineDefinition(i + 1, acuity, letters, capHeightDips, bottomSpacing));
            nextLineCapHeight = capHeightDips;
        }

        return lines;
    }

    private IReadOnlyList<string> ResolveLineLetters(LetterChartOptions options, IReadOnlyList<int> letterCounts, LetterSequenceGenerator generator)
    {
        var lines = new List<string>(options.NumberOfLines);
        for (var i = 0; i < options.NumberOfLines; i++)
        {
            if (options.ExplicitLineLetters != null && i < options.ExplicitLineLetters.Count)
            {
                var cleaned = new string(options.ExplicitLineLetters[i]
                    .Where(char.IsLetter)
                    .Select(char.ToUpperInvariant)
                    .ToArray());

                if (cleaned.Length == 0)
                {
                    cleaned = generator.GenerateLine(letterCounts[i], options);
                }
                else if (cleaned.Length < letterCounts[i])
                {
                    cleaned += generator.GenerateLine(letterCounts[i] - cleaned.Length, options);
                }
                else if (cleaned.Length > letterCounts[i])
                {
                    cleaned = cleaned.Substring(0, letterCounts[i]);
                }

                lines.Add(cleaned);
            }
            else
            {
                lines.Add(generator.GenerateLine(letterCounts[i], options));
            }
        }

        return lines;
    }

    private IReadOnlyList<LetterDefinition> CreateLetters(string letters, int desiredCount, double capHeightDips, LetterChartOptions options, Random random)
    {
        var list = new List<LetterDefinition>(desiredCount);
        var spacing = capHeightDips;
        for (var i = 0; i < desiredCount; i++)
        {
            var glyph = letters[i % letters.Length];
            var rotation = options.Variant switch
            {
                ChartVariant.Sloan => 0,
                ChartVariant.TumblingE => PickRotation(random, new[] { 0, 90, 180, 270 }),
                ChartVariant.TumblingC => PickRotation(random, new[] { 0, 45, 90, 135, 180, 225, 270, 315 }),
                _ => 0
            };

            list.Add(new LetterDefinition(glyph, capHeightDips, spacing, rotation));
        }

        return list;
    }

    private double PickRotation(Random random, IReadOnlyList<int> angles)
    {
        return angles[random.Next(angles.Count)];
    }
}
