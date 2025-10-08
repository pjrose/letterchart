using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LetterChartCore.Models;
using LetterChartCore.Services;

namespace LetterChartApp.Views;

public partial class LetterChartWindow : Window
{
    private readonly ChartLayoutBuilder _layoutBuilder = new();
    private readonly Dictionary<int, List<TextBlock>> _lineTextBlocks = new();
    private readonly List<StackPanel> _letterPanels = new();
    private readonly List<(FrameworkElement Left, FrameworkElement Right)> _leaderElements = new();
    private Brush _defaultBrush = Brushes.Black;
    private FontFamily _fontFamily = new("Arial");
    private FontWeight _fontWeight = FontWeights.Normal;
    private double _capHeightRatio = 0.7;

    public LetterChartWindow()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    public void InitializeChart(LetterChartOptions options)
    {
        _lineTextBlocks.Clear();
        _letterPanels.Clear();
        _leaderElements.Clear();
        ChartGrid.Children.Clear();
        ChartGrid.RowDefinitions.Clear();

        _defaultBrush = CreateBrush(options.FontColor);
        Background = new SolidColorBrush(ToMediaColor(options.BackgroundColor));
        RootGrid.Background = new SolidColorBrush(ToMediaColor(options.BackgroundColor));

        ConfigureTypography(options.FontFamilyName);

        var lines = _layoutBuilder.Build(options);
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            ChartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var lineGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, i == lines.Count - 1 ? 0 : line.BottomSpacing)
            };
            lineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            lineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            lineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto});

            var lettersPanel = CreateLettersPanel();
            _letterPanels.Add(lettersPanel);

            var textBlocks = CreateLetterBlocks(line, lettersPanel);
            _lineTextBlocks[line.LineNumber] = textBlocks;

            var leaderLineLength = options.LeaderLineLengthInches * 96.0;
            var leftLeader = CreateLeader(line, leaderLineLength, options, true);
            var rightLeader = CreateLeader(line, leaderLineLength, options, false);
            _leaderElements.Add((leftLeader, rightLeader));
            UpdateLeaderVisibility(options.ShowLeaderLines, leftLeader, rightLeader);

            lineGrid.Children.Add(leftLeader);
            Grid.SetColumn(leftLeader, 0);
            lineGrid.Children.Add(lettersPanel);
            Grid.SetColumn(lettersPanel, 1);
            lineGrid.Children.Add(rightLeader);
            Grid.SetColumn(rightLeader, 2);

            ChartGrid.Children.Add(lineGrid);
            Grid.SetRow(lineGrid, i);       
        }

        UpdateLineWidths();
    }

    public void SetLineColor(int lineNumber, Color? color = null)
    {
        if (_lineTextBlocks.TryGetValue(lineNumber, out var blocks))
        {
            var brush = new SolidColorBrush(color ?? Colors.Red);
            foreach (var block in blocks)
            {
                block.Foreground = brush;
            }
        }
    }

    public void SetLetterColor(int lineNumber, int letterIndex, Color? color = null)
    {
        if (_lineTextBlocks.TryGetValue(lineNumber, out var blocks) && letterIndex >= 0 && letterIndex < blocks.Count)
        {
            blocks[letterIndex].Foreground = new SolidColorBrush(color ?? Colors.Red);
        }
    }

    public void ResetColors()
    {
        foreach (var blocks in _lineTextBlocks.Values)
        {
            foreach (var block in blocks)
            {
                block.Foreground = _defaultBrush;
            }
        }
    }

    public void SetLeaderVisibility(bool isVisible)
    {
        foreach (var (left, right) in _leaderElements)
        {
            UpdateLeaderVisibility(isVisible, left, right);
        }
    }

    private void UpdateLeaderVisibility(bool isVisible, FrameworkElement left, FrameworkElement right)
    {
        left.Visibility = right.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private static StackPanel CreateLettersPanel()
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0),
            Background = Brushes.Transparent
        };
    }

    private List<TextBlock> CreateLetterBlocks(LetterLineDefinition line, Panel panel)
    {
        var blocks = new List<TextBlock>(line.Letters.Count);
        var letters = line.Letters;
        var letterCount = letters.Count;
        for (var i = 0; i < letterCount; i++)
        {
            var letter = letters[i];
            var margin = new Thickness(letter.LetterSpacing / 2, 0, letter.LetterSpacing / 2, 0);
            if (i == 0)
            {
                margin.Left = 0;
            }
            if (i == letterCount - 1)
            {
                margin.Right = 0;
            }

            var fontSize = _capHeightRatio > 0 ? letter.TargetCapHeight / _capHeightRatio : letter.TargetCapHeight;
            var textBlock = new TextBlock
            {
                Text = letter.Glyph.ToString(),
                FontFamily = _fontFamily,
                FontWeight = _fontWeight,
                FontSize = fontSize,
                Foreground = _defaultBrush,
                Margin = margin,
                TextAlignment = TextAlignment.Center,
                LayoutTransform = CreateTransform(letter.RotationDegrees),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            panel.Children.Add(textBlock);
            blocks.Add(textBlock);
        }

        return blocks;
    }

    private static Transform CreateTransform(double rotation)
    {
        if (Math.Abs(rotation) < 0.01)
        {
            return Transform.Identity;
        }

        return new RotateTransform(rotation);
    }

    private FrameworkElement CreateLeader(LetterLineDefinition line, double length, LetterChartOptions options, bool isLeft)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = isLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right,
            // reduce the outer inset so the leader renders closer to the window edge
            Margin = isLeft ? new Thickness(8, 0, 20, 0) : new Thickness(20, 0, 8, 0)
        };

        var lineVisual = new Border
        {
            Background = _defaultBrush,
            Width = length,
            Height = 2,
            VerticalAlignment = VerticalAlignment.Center
        };

        var text = new TextBlock
        {
            Text = $"{line.LineNumber} {line.Acuity}",
            Foreground = _defaultBrush,
            FontSize = options.LeaderFontSize,
            Margin = new Thickness(8, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        if (isLeft)
        {
            stack.Children.Add(lineVisual);
            stack.Children.Add(text);
        }
        else
        {
            text.Margin = new Thickness(0, 0, 8, 0);
            stack.Children.Add(text);
            stack.Children.Add(lineVisual);
        }

        return stack;
    }

    private void ConfigureTypography(string fontFamilyName)
    {
        var requested = new FontFamily(fontFamilyName);
        var typeface = new Typeface(requested, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out var glyphTypeface))
        {
            typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            typeface.TryGetGlyphTypeface(out glyphTypeface);
        }

        _fontFamily = typeface.FontFamily;
        _fontWeight = typeface.Weight;
        _capHeightRatio = glyphTypeface?.CapsHeight ?? 0.7;
        if (_capHeightRatio <= 0)
        {
            _capHeightRatio = 0.7;
        }
    }

    private static Brush CreateBrush(ChartColor color)
    {
        return new SolidColorBrush(ToMediaColor(color));
    }

    private static Color ToMediaColor(ChartColor color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateLineWidths();
    }

    private void UpdateLineWidths()
    {
        if (_letterPanels.Count == 0)
        {
            return;
        }

        var maxWidth = ActualWidth * 2.0 / 3.0;
        foreach (var panel in _letterPanels)
        {
            panel.MaxWidth = maxWidth;
        }
    }
}
