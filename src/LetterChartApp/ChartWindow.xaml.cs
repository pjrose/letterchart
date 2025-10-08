using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LetterChartApp.ViewModels;

namespace LetterChartApp;

public partial class ChartWindow : Window
{
    public ChartWindow(ChartViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += OnLoaded;
        PreviewKeyDown += OnPreviewKeyDown;
    }

    public ChartViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var width = SystemParameters.PrimaryScreenWidth * 0.66;
        ViewModel.SetMaxChartWidth(width);
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    public void SetLineColor(int lineIndex, Color color) => ViewModel.SetLineColor(lineIndex, color);

    public void SetLetterColor(int lineIndex, int letterIndex, Color color)
        => ViewModel.SetLetterColor(lineIndex, letterIndex, color);

    public void HighlightLine(int lineIndex) => SetLineColor(lineIndex, Colors.Red);

    public void HighlightLetter(int lineIndex, int letterIndex)
        => SetLetterColor(lineIndex, letterIndex, Colors.Red);

    public void ResetColors() => ViewModel.ResetColors();

    public void SetLeaderLinesVisible(bool show) => ViewModel.SetLeaderLinesVisibility(show);
}
