using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using LetterChart.Core;

namespace LetterChart.Rendering;

public sealed class LetterChartControl : FrameworkElement
{
    private static readonly Typeface DefaultLeaderTypeface = new(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

    private IReadOnlyList<LineVisualState>? _lines;
    private Typeface _chartTypeface = new(new FontFamily("Sloan"), FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal);
    private Brush _backgroundBrush = Brushes.White;
    private Brush _leaderLineBrush = Brushes.Gray;
    private bool _showLeaderLines = true;

    public Brush BackgroundBrush
    {
        get => _backgroundBrush;
        set
        {
            _backgroundBrush = value ?? Brushes.White;
            InvalidateVisual();
        }
    }

    public Brush LeaderLineBrush
    {
        get => _leaderLineBrush;
        set
        {
            _leaderLineBrush = value ?? Brushes.Gray;
            InvalidateVisual();
        }
    }

    public bool ShowLeaderLines
    {
        get => _showLeaderLines;
        set
        {
            if (_showLeaderLines != value)
            {
                _showLeaderLines = value;
                InvalidateVisual();
            }
        }
    }

    public Typeface ChartTypeface
    {
        get => _chartTypeface;
        set
        {
            _chartTypeface = value ?? throw new ArgumentNullException(nameof(value));
            InvalidateVisual();
        }
    }

    public Typeface LeaderTypeface { get; set; } = DefaultLeaderTypeface;

    public double LeaderFontSize { get; set; } = 16.0;

    public double LeaderLineLengthDip { get; set; } = MeasurementUtilities.InchesToDeviceIndependentPixels(2.0);

    public double LeaderMarginDip { get; set; } = MeasurementUtilities.InchesToDeviceIndependentPixels(0.5);

    public double LetterSpacingFactor { get; set; } = 1.0;

    public double LineSpacingFactor { get; set; } = 1.0;

    public void SetLines(IReadOnlyList<LineVisualState> lines)
    {
        _lines = lines;
        InvalidateVisual();
    }

    public void RequestRender()
    {
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
        drawingContext.DrawRectangle(BackgroundBrush, null, bounds);

        if (_lines is null || _lines.Count == 0)
        {
            return;
        }

        var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
        var lineHeights = new double[_lines.Count];
        for (var i = 0; i < _lines.Count; i++)
        {
            lineHeights[i] = MeasurementUtilities.MillimetresToDeviceIndependentPixels(_lines[i].Definition.LetterHeightMillimeters);
        }

        var totalHeight = 0.0;
        for (var i = 0; i < lineHeights.Length; i++)
        {
            totalHeight += lineHeights[i];
            if (i < lineHeights.Length - 1)
            {
                totalHeight += lineHeights[i + 1] * LineSpacingFactor;
            }
        }

        var startY = (ActualHeight - totalHeight) / 2.0;
        var chartWidth = ActualWidth * (2.0 / 3.0);
        var chartLeft = (ActualWidth - chartWidth) / 2.0;

        var y = startY;
        for (var lineIndex = 0; lineIndex < _lines.Count; lineIndex++)
        {
            var line = _lines[lineIndex];
            var letterHeight = lineHeights[lineIndex];
            var letterSpacing = letterHeight * LetterSpacingFactor;
            var letterCount = line.Letters.Count;
            var lineWidth = letterCount * letterHeight + Math.Max(0, letterCount - 1) * letterSpacing;
            var startX = chartLeft + Math.Max(0, (chartWidth - lineWidth) / 2.0);

            var letterY = y;
            for (var letterIndex = 0; letterIndex < letterCount; letterIndex++)
            {
                var letterState = line.Letters[letterIndex];
                var letterX = startX + letterIndex * (letterHeight + letterSpacing);
                DrawLetter(drawingContext, letterState, letterX, letterY, letterHeight, pixelsPerDip, line.LineBrushOverride ?? letterState.Brush);
            }

            if (ShowLeaderLines)
            {
                DrawLeaderLines(drawingContext, lineIndex, line, letterHeight, y + letterHeight / 2.0, pixelsPerDip);
            }

            y += letterHeight;
            if (lineIndex < _lines.Count - 1)
            {
                y += lineHeights[lineIndex + 1] * LineSpacingFactor;
            }
        }
    }

    private void DrawLetter(DrawingContext context, LetterVisualState state, double x, double y, double letterHeight, double pixelsPerDip, Brush brush)
    {
        var formatted = new FormattedText(state.Symbol.Symbol.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, ChartTypeface, letterHeight, brush, pixelsPerDip)
        {
            TextAlignment = TextAlignment.Center
        };

        var center = new Point(x + letterHeight / 2.0, y + letterHeight / 2.0);
        context.PushTransform(new TranslateTransform(center.X, center.Y));
        if (Math.Abs(state.Symbol.RotationDegrees) > double.Epsilon)
        {
            context.PushTransform(new RotateTransform(state.Symbol.RotationDegrees));
        }

        context.DrawText(formatted, new Point(-formatted.WidthIncludingTrailingWhitespace / 2.0, -formatted.Height / 2.0));

        if (Math.Abs(state.Symbol.RotationDegrees) > double.Epsilon)
        {
            context.Pop();
        }

        context.Pop();
    }

    private void DrawLeaderLines(DrawingContext context, int lineIndex, LineVisualState line, double letterHeight, double centerY, double pixelsPerDip)
    {
        var pen = new Pen(LeaderLineBrush, 1.5);
        var leftStart = new Point(LeaderMarginDip, centerY);
        var leftEnd = new Point(LeaderMarginDip + LeaderLineLengthDip, centerY);
        context.DrawLine(pen, leftStart, leftEnd);

        var rightEnd = new Point(ActualWidth - LeaderMarginDip - LeaderLineLengthDip, centerY);
        var rightStart = new Point(ActualWidth - LeaderMarginDip, centerY);
        context.DrawLine(pen, rightEnd, rightStart);

        var label = $"{lineIndex + 1} {line.Definition.Acuity.Display}";
        var labelBrush = LeaderLineBrush;
        var text = new FormattedText(label, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, LeaderTypeface, LeaderFontSize, labelBrush, pixelsPerDip);
        context.DrawText(text, new Point(leftEnd.X + 4, centerY - text.Height / 2.0));
        context.DrawText(text, new Point(rightEnd.X - text.Width - 4, centerY - text.Height / 2.0));
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        InvalidateVisual();
    }
}
