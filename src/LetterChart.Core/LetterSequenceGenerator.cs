using System;
using System.Collections.Generic;
using System.Linq;

namespace LetterChart.Core;

public sealed class LetterSequenceGenerator
{
    private readonly IReadOnlyList<char> _allowed;
    private readonly IReadOnlyList<char> _preferred;
    private readonly Random _random;

    public LetterSequenceGenerator(ISet<char> allowed, ISet<char> preferred, int? seed = null)
    {
        if (allowed.Count == 0)
        {
            throw new ArgumentException("At least one allowed letter is required.", nameof(allowed));
        }

        _allowed = allowed.Select(char.ToUpperInvariant).Distinct().OrderBy(c => c).ToArray();
        _preferred = preferred.Count == 0
            ? Array.Empty<char>()
            : preferred.Select(char.ToUpperInvariant).Distinct().Where(c => allowed.Contains(c)).OrderBy(c => c).ToArray();
        _random = seed.HasValue ? new Random(seed.Value) : Random.Shared;
    }

    public string Generate(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        Span<char> buffer = count <= 128 ? stackalloc char[count] : new char[count];
        for (var i = 0; i < count; i++)
        {
            buffer[i] = NextLetter();
        }

        return new string(buffer);
    }

    private char NextLetter()
    {
        if (_preferred.Count == 0)
        {
            return _allowed[_random.Next(_allowed.Count)];
        }

        var usePreferred = _random.NextDouble() < 0.7; // Bias toward Sloan letters.
        var source = usePreferred ? _preferred : _allowed;
        return source[_random.Next(source.Count)];
    }
}
