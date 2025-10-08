using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using LetterChart.Core;
using LetterChartApp;

namespace LetterChartApp.ViewModels;

public sealed class LetterLineViewModel : ObservableObject
{
    private readonly Brush _defaultBrush;
    private Brush _lineForeground;

    public LetterLineViewModel(int lineNumber, LetterLineDefinition definition, Brush brush, bool isLastLine)
    {
        LineNumber = lineNumber;
        Acuity = definition.Acuity;
        FontSize = definition.FontSize;
        LineMargin = CreateLineMargin(lineNumber, definition.LineSpacing, isLastLine);
        LeaderMargin = LineMargin;
        _defaultBrush = brush;
        _lineForeground = brush;
        Letters = CreateLetters(definition, brush);
        LeaderText = $"{lineNumber} {definition.Acuity}";
    }

    public int LineNumber { get; }

    public SnellenFraction Acuity { get; }

    public double FontSize { get; }

    public Thickness LineMargin { get; }

    public Thickness LeaderMargin { get; }

    public string LeaderText { get; }

    public ObservableCollection<LetterCellViewModel> Letters { get; }

    public Brush LineForeground
    {
        get => _lineForeground;
        private set => SetProperty(ref _lineForeground, value);
    }

    public void SetLineForeground(Brush brush)
    {
        LineForeground = brush;
        foreach (var letter in Letters)
        {
            letter.SetForeground(brush);
        }
    }

    public void SetLetterForeground(int index, Brush brush)
    {
        if (index < 0 || index >= Letters.Count)
        {
            return;
        }

        Letters[index].SetForeground(brush);
    }

    public void Reset()
    {
        LineForeground = _defaultBrush;
        foreach (var letter in Letters)
        {
            letter.ResetForeground();
        }
    }

    private static Thickness CreateLineMargin(int lineNumber, double lineSpacing, bool isLastLine)
    {
        var spacing = lineSpacing / 2.0;
        var top = lineNumber == 1 ? 0 : spacing;
        var bottom = isLastLine ? 0 : spacing;
        return new Thickness(0, top, 0, bottom);
    }

    private ObservableCollection<LetterCellViewModel> CreateLetters(LetterLineDefinition definition, Brush brush)
    {
        var marginValue = definition.LetterSpacing / 2.0;
        var margin = new Thickness(marginValue, 0, marginValue, 0);
        var collection = new ObservableCollection<LetterCellViewModel>();

        for (var i = 0; i < definition.Letters.Length; i++)
        {
            collection.Add(new LetterCellViewModel(definition.Letters[i], definition.Rotations[i], margin, brush));
        }

        return collection;
    }
}
