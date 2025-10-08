using System;
using System.Collections.Generic;
using System.Linq;

namespace LetterChart.Core;

public sealed class LetterChartBuilder
{
    public LetterChartDefinition BuildChart(LetterChartOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        var lettersPerLine = options.GetLettersPerLine();
        var generator = new LetterSequenceGenerator(options.AllowedLetters, options.PreferredLetters);
        var lines = new List<LetterLineDefinition>(options.NumberOfLines);

        for (var i = 0; i < options.NumberOfLines; i++)
        {
            var acuity = options.AcuityTargets[i];
            var letterCount = lettersPerLine[i];
            var letters = options.LetterSequences is { } sequences
                ? sequences[i]
                : generator.Generate(letterCount);

            letters = new string(letters
                .Where(char.IsLetter)
                .Select(char.ToUpperInvariant)
                .Take(letterCount)
                .ToArray());

            if (letters.Length != letterCount)
            {
                letters = letters.PadRight(letterCount, 'E');
            }

            var letterHeight = SnellenMath.GetLetterHeightPixels(
                options.ViewingDistanceMeters,
                options.PixelPitchMillimeters,
                options.ScaleFactor,
                acuity);

            var lineSpacing = SnellenMath.GetLineSpacingPixels(letterHeight);
            var letterSpacing = SnellenMath.GetLetterSpacingPixels(letterHeight);

            var rotations = Enumerable.Repeat(0d, letters.Length).ToArray();

            lines.Add(new LetterLineDefinition(
                acuity,
                letters,
                letterHeight,
                letterSpacing,
                lineSpacing,
                rotations));
        }

        return new LetterChartDefinition(lines, options.FontColor, options.BackgroundColor, options.ShowLeaderLines);
    }

    public LetterChartDefinition BuildTumblingChart(LetterChartOptions options, char symbol)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        if (!char.IsLetter(symbol))
        {
            throw new ArgumentException("Symbol must be a letter.", nameof(symbol));
        }

        symbol = char.ToUpperInvariant(symbol);

        var lettersPerLine = options.GetLettersPerLine();
        var random = Random.Shared;
        var lines = new List<LetterLineDefinition>(options.NumberOfLines);
        var possibleAngles = new[] { 0d, 90d, 180d, 270d };

        for (var i = 0; i < options.NumberOfLines; i++)
        {
            var acuity = options.AcuityTargets[i];
            var letterCount = lettersPerLine[i];
            var letters = new char[letterCount];
            var rotations = new double[letterCount];

            for (var j = 0; j < letterCount; j++)
            {
                letters[j] = symbol;
                rotations[j] = possibleAngles[random.Next(possibleAngles.Length)];
            }

            var letterHeight = SnellenMath.GetLetterHeightPixels(
                options.ViewingDistanceMeters,
                options.PixelPitchMillimeters,
                options.ScaleFactor,
                acuity);

            var lineSpacing = SnellenMath.GetLineSpacingPixels(letterHeight);
            var letterSpacing = SnellenMath.GetLetterSpacingPixels(letterHeight);

            lines.Add(new LetterLineDefinition(
                acuity,
                new string(letters),
                letterHeight,
                letterSpacing,
                lineSpacing,
                rotations));
        }

        return new LetterChartDefinition(lines, options.FontColor, options.BackgroundColor, options.ShowLeaderLines);
    }
}
