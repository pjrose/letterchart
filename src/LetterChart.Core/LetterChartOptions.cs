using System;
using System.Collections.Generic;
using System.Linq;

namespace LetterChart.Core;

public sealed class LetterChartOptions
{
    private static readonly SnellenFraction[] DefaultAcuityTargets =
    {
        new(20, 20),
        new(20, 30),
        new(20, 40)
    };

    public double ViewingDistanceMeters { get; init; } = 4.0;

    public double PixelPitchMillimeters { get; init; } = 0.155;

    public double ScaleFactor { get; init; } = 1.0;

    public int NumberOfLines { get; init; } = 3;

    public IReadOnlyList<SnellenFraction> AcuityTargets { get; init; } = DefaultAcuityTargets;

    public IReadOnlyList<int>? LettersPerLine { get; init; }
        = null; // null => infer based on acuity count.

    public IReadOnlyList<string>? LetterSequences { get; init; }
        = null; // null => generator chooses letters.

    public ISet<char> PreferredLetters { get; init; } = new HashSet<char>("CDEFLNOPTZ");

    public ISet<char> AllowedLetters { get; init; } = new HashSet<char>("ABCDEFGHJKLMNOPQRSTUVWXYZ");

    public string FontColor { get; init; } = "#000000";

    public string BackgroundColor { get; init; } = "#FFFFFF";

    public bool ShowLeaderLines { get; init; } = true;

    public void Validate()
    {
        if (ViewingDistanceMeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ViewingDistanceMeters));
        }

        if (PixelPitchMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(PixelPitchMillimeters));
        }

        if (ScaleFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ScaleFactor));
        }

        if (NumberOfLines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(NumberOfLines));
        }

        if (AcuityTargets.Count < NumberOfLines)
        {
            throw new ArgumentException("Not enough acuity targets for the requested number of lines.", nameof(AcuityTargets));
        }

        if (LetterSequences is not null && LetterSequences.Count < NumberOfLines)
        {
            throw new ArgumentException("Not enough letter sequences for the requested number of lines.", nameof(LetterSequences));
        }

        if (LettersPerLine is not null && LettersPerLine.Count < NumberOfLines)
        {
            throw new ArgumentException("Not enough letter-count entries for the requested number of lines.", nameof(LettersPerLine));
        }

        if (AllowedLetters.Count == 0)
        {
            throw new ArgumentException("At least one allowed letter must be provided.", nameof(AllowedLetters));
        }
    }

    public IReadOnlyList<int> GetLettersPerLine()
    {
        if (LettersPerLine is not null)
        {
            return LettersPerLine.Take(NumberOfLines).ToArray();
        }

        return Enumerable.Repeat(5, NumberOfLines).ToArray();
    }
}
