using System;
using System.Collections.Generic;
using System.Linq;
using LetterChart.Core.Models;

namespace LetterChart.Core;

/// <summary>
/// Parameters that describe how a chart should be generated.
/// </summary>
public sealed record class ChartParameters
{
    private static readonly IReadOnlyList<AcuityValue> DefaultAcuities = new[]
    {
        new AcuityValue(20, 20),
        new AcuityValue(20, 30),
        new AcuityValue(20, 40)
    };

    public int NumberOfLines { get; init; } = 3;

    public double ScreenDistanceMeters { get; init; } = 4.0;

    public double PixelPitchMillimeters { get; init; } = 0.155;

    /// <summary>
    /// Scale applied to the pixel pitch (percent). 100 means no change.
    /// </summary>
    public double ScalePercent { get; init; } = 100.0;

    public IReadOnlyList<AcuityValue>? AcuityTargets { get; init; }
        = DefaultAcuities;

    /// <summary>
    /// Optional explicit letter sequences per line. Each entry should already be uppercase.
    /// </summary>
    public IReadOnlyList<string>? LetterSequences { get; init; }
        = null;

    /// <summary>
    /// When true (default) the generator randomises Sloan letters when no explicit sequence is supplied.
    /// </summary>
    public bool RandomiseLetters { get; init; } = true;

    public ChartSymbolMode SymbolMode { get; init; } = ChartSymbolMode.SloanLetters;

    /// <summary>
    /// When provided, overrides the default letter counts per line.
    /// </summary>
    public IReadOnlyList<int>? LetterCounts { get; init; }
        = null;

    /// <summary>
    /// Optional seed for deterministic generation.
    /// </summary>
    public int? RandomSeed { get; init; }
        = null;

    internal IReadOnlyList<AcuityValue> ResolveAcuities()
    {
        var list = AcuityTargets is null || AcuityTargets.Count == 0 ? DefaultAcuities : AcuityTargets;
        var count = Math.Max(1, NumberOfLines);
        if (list.Count >= count)
        {
            return list.Take(count).ToArray();
        }

        // Repeat the last acuity if too few were provided.
        var result = new List<AcuityValue>(count);
        result.AddRange(list);
        while (result.Count < count)
        {
            result.Add(list[^1]);
        }

        return result;
    }

    internal IReadOnlyList<int> ResolveLetterCounts()
    {
        if (LetterCounts is { Count: > 0 })
        {
            return LetterCounts.Select(c => Math.Max(1, c)).Take(NumberOfLines).ToArray();
        }

        // ANSI Z80.21 typical pattern (1,2,3,4,5...) - ensure enough entries
        var counts = new List<int>();
        for (var i = 0; i < NumberOfLines; i++)
        {
            counts.Add(Math.Min(5, i + 1));
        }

        return counts;
    }

    internal double EffectivePixelPitch => PixelPitchMillimeters * (ScalePercent / 100.0);
}
