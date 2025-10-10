
using LetterChartCore.Models;

namespace LetterChartCore.Services;

/// <summary>
/// Provides repeatable generation of optotype letters.
/// </summary>
public sealed class LetterSequenceGenerator
{
    private readonly Random _random;

    public LetterSequenceGenerator(int? seed)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public string GenerateLine(int count, LetterChartOptions options)
    {
        return options.Variant switch
        {
            ChartVariant.Sloan => GenerateSloanLine(count, options),
            ChartVariant.TumblingE => new string('E', count),
            ChartVariant.TumblingC => new string('C', count),
            _ => throw new ArgumentOutOfRangeException(nameof(options.Variant), options.Variant, null)
        };
    }

    private string GenerateSloanLine(int count, LetterChartOptions options)
    {
        var result = new char[count];
        var previous = '\0';
        for (var i = 0; i < count; i++)
        {
            var candidate = GetCandidateLetter(options);
            var attempts = 0;
            while (candidate == previous && attempts < 4)
            {
                candidate = GetCandidateLetter(options);
                attempts++;
            }

            result[i] = candidate;
            previous = candidate;
        }

        return new string(result);
    }

    private char GetCandidateLetter(LetterChartOptions options)
    {
        var preferredProbability = 0.75;
        var usePreferred = _random.NextDouble() < preferredProbability && options.PreferredLetters.Count > 0;
        var pool = usePreferred ? options.PreferredLetters : options.SupplementalLetters;
        if (pool.Count == 0)
        {
            pool = options.PreferredLetters;
        }

        if (pool.Count == 0)
        {
            return 'E';
        }

        var index = _random.Next(pool.Count);
        return char.ToUpperInvariant(pool[index]);
    }
}
