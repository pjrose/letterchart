using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LetterChart.Core;
using LetterChart.Core.Models;
using LetterChart.Rendering;

namespace LetterChart;

public partial class LetterChartWindow : Window
{
    private ChartParameters _parameters;
    private readonly LetterChartViewOptions _options;
    private readonly List<LineVisualState> _lineStates;
    private readonly SolidColorBrush _defaultBrush;
    private readonly SolidColorBrush _backgroundBrush;
    private readonly SolidColorBrush _leaderBrush;
    private ChartDefinition _chartDefinition;

    public LetterChartWindow(ChartParameters parameters, LetterChartViewOptions? viewOptions = null)
    {
        InitializeComponent();

        _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        _options = viewOptions ?? LetterChartViewOptions.CreateDefault();

        _defaultBrush = CreateFrozenBrush(_options.FontColor);
        _backgroundBrush = CreateFrozenBrush(_options.BackgroundColor);
        _leaderBrush = CreateFrozenBrush(_options.LeaderLineColor);

        _chartDefinition = ChartGenerator.GenerateChart(_parameters);
        _lineStates = _chartDefinition.Lines.Select(line => new LineVisualState(line, _defaultBrush)).ToList();

        ConfigureWindowAppearance();
    }

    public ChartDefinition CurrentChart => _chartDefinition;

    public void SetLeaderLinesVisible(bool isVisible)
    {
        ChartControl.ShowLeaderLines = isVisible;
        ChartControl.RequestRender();
    }

    public void SetLineColor(int lineIndex, Color? color = null)
    {
        var line = GetLine(lineIndex);
        line.LineBrushOverride = CreateFrozenBrush(color ?? Colors.Red);
        ChartControl.RequestRender();
    }

    public void SetLetterColor(int lineIndex, int letterIndex, Color? color = null)
    {
        var line = GetLine(lineIndex);
        if (letterIndex < 0 || letterIndex >= line.Letters.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(letterIndex));
        }

        line.Letters[letterIndex].Brush = CreateFrozenBrush(color ?? Colors.Red);
        ChartControl.RequestRender();
    }

    public void ResetColors()
    {
        foreach (var line in _lineStates)
        {
            line.ResetColors();
        }

        ChartControl.RequestRender();
    }

    public void RegenerateChart(ChartParameters parameters)
    {
        _parameters = parameters;
        _chartDefinition = ChartGenerator.GenerateChart(parameters);
        _lineStates.Clear();
        foreach (var line in _chartDefinition.Lines.Select(l => new LineVisualState(l, _defaultBrush)))
        {
            _lineStates.Add(line);
        }

        ChartControl.SetLines(_lineStates);
    }

    private void ConfigureWindowAppearance()
    {
        Background = _backgroundBrush;
        ChartControl.BackgroundBrush = _backgroundBrush;
        ChartControl.LeaderLineBrush = _leaderBrush;
        ChartControl.ShowLeaderLines = _options.ShowLeaderLines;
        ChartControl.LetterSpacingFactor = _options.LetterSpacingFactor;
        ChartControl.LineSpacingFactor = _options.LineSpacingFactor;
        ChartControl.LeaderFontSize = _options.LeaderFontSize;
        ChartControl.LeaderLineLengthDip = MeasurementUtilities.InchesToDeviceIndependentPixels(_options.LeaderLineLengthInches);
        ChartControl.LeaderMarginDip = MeasurementUtilities.InchesToDeviceIndependentPixels(_options.LeaderMarginInches);

        var fontFamily = new FontFamily($"{_options.FontFamilyName}, Arial, Helvetica, Sans-Serif");
        ChartControl.ChartTypeface = new Typeface(fontFamily, FontStyles.Normal, _options.FontWeight, FontStretches.Normal);
        ChartControl.SetLines(_lineStates);
    }

    private LineVisualState GetLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= _lineStates.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(lineIndex));
        }

        return _lineStates[lineIndex];
    }

    private static SolidColorBrush CreateFrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        if (!brush.IsFrozen && brush.CanFreeze)
        {
            brush.Freeze();
        }

        return brush;
    }
}
