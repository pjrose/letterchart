using System.Windows.Media;
using LetterChart.Core.Models;

namespace LetterChart.Rendering;

internal sealed class LetterVisualState
{
    public LetterVisualState(ChartLetterSymbol symbol, Brush brush)
    {
        Symbol = symbol;
        Brush = brush;
    }

    public ChartLetterSymbol Symbol { get; }

    public Brush Brush { get; set; }
}
