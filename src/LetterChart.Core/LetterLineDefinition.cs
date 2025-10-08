using System.Collections.Generic;

namespace LetterChart.Core;

public sealed record LetterLineDefinition(
    SnellenFraction Acuity,
    string Letters,
    double FontSize,
    double LetterSpacing,
    double LineSpacing,
    IReadOnlyList<double> Rotations);

public sealed record LetterChartDefinition(
    IReadOnlyList<LetterLineDefinition> Lines,
    string FontColor,
    string BackgroundColor,
    bool ShowLeaderLines);
