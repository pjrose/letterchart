using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media;
using LetterChart.Core;
using LetterChartApp;

namespace LetterChartApp.ViewModels;

public sealed class ChartViewModel : ObservableObject
{
    private double _maxChartWidth;
    private bool _showLeaderLines;

    public ChartViewModel(LetterChartDefinition definition, string fontFamilyName)
    {
        ArgumentNullException.ThrowIfNull(definition);
        FontFamily = new FontFamily(fontFamilyName);
        BackgroundBrush = CreateBrush(definition.BackgroundColor);
        LetterBrush = CreateBrush(definition.FontColor);
        LeaderBrush = LetterBrush;
        LeaderFontSize = 18;
        LeaderLineLength = 192; // Approximately two inches at 96 DPI.
        _showLeaderLines = definition.ShowLeaderLines;
        Lines = CreateLines(definition, LetterBrush);
    }

    public ObservableCollection<LetterLineViewModel> Lines { get; }

    public FontFamily FontFamily { get; }

    public Brush BackgroundBrush { get; }

    public Brush LetterBrush { get; }

    public Brush LeaderBrush { get; }

    public double LeaderFontSize { get; }

    public double LeaderLineLength { get; }

    public double MaxChartWidth
    {
        get => _maxChartWidth;
        private set => SetProperty(ref _maxChartWidth, value);
    }

    public bool ShowLeaderLines
    {
        get => _showLeaderLines;
        private set => SetProperty(ref _showLeaderLines, value);
    }

    public void SetMaxChartWidth(double width)
    {
        if (width <= 0)
        {
            return;
        }

        MaxChartWidth = width;
    }

    public void SetLeaderLinesVisibility(bool visible) => ShowLeaderLines = visible;

    public void SetLineColor(int lineIndex, Color color)
    {
        if (lineIndex < 0 || lineIndex >= Lines.Count)
        {
            return;
        }

        var brush = new SolidColorBrush(color);
        brush.Freeze();
        Lines[lineIndex].SetLineForeground(brush);
    }

    public void SetLetterColor(int lineIndex, int letterIndex, Color color)
    {
        if (lineIndex < 0 || lineIndex >= Lines.Count)
        {
            return;
        }

        var brush = new SolidColorBrush(color);
        brush.Freeze();
        Lines[lineIndex].SetLetterForeground(letterIndex, brush);
    }

    public void ResetColors()
    {
        foreach (var line in Lines)
        {
            line.Reset();
        }
    }

    private ObservableCollection<LetterLineViewModel> CreateLines(LetterChartDefinition definition, Brush brush)
    {
        var collection = new ObservableCollection<LetterLineViewModel>();
        for (var i = 0; i < definition.Lines.Count; i++)
        {
            var isLast = i == definition.Lines.Count - 1;
            collection.Add(new LetterLineViewModel(i + 1, definition.Lines[i], brush, isLast));
        }

        return collection;
    }

    private static Brush CreateBrush(string color)
    {
        var converter = new BrushConverter();
        var brush = (Brush)converter.ConvertFromString(null, CultureInfo.InvariantCulture, color)!;
        if (brush is SolidColorBrush solid)
        {
            solid.Freeze();
        }

        return brush;
    }
}
