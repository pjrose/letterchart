using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LetterChart.Core.Models;

namespace LetterChart.Rendering;

internal sealed class LineVisualState
{
    public LineVisualState(ChartLineDefinition definition, Brush defaultBrush)
    {
        Definition = definition;
        DefaultBrush = defaultBrush;
        Letters = definition.Letters.Select(symbol => new LetterVisualState(symbol, defaultBrush)).ToList();
    }

    public ChartLineDefinition Definition { get; }

    public Brush DefaultBrush { get; }

    public Brush? LineBrushOverride { get; set; }

    public List<LetterVisualState> Letters { get; }

    public void ResetColors()
    {
        LineBrushOverride = null;
        foreach (var letter in Letters)
        {
            letter.Brush = DefaultBrush;
        }
    }
}
