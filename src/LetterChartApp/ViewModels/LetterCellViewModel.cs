using System.Windows;
using System.Windows.Media;
using LetterChartApp;

namespace LetterChartApp.ViewModels;

public sealed class LetterCellViewModel : ObservableObject
{
    private Brush _foreground;

    public LetterCellViewModel(char character, double rotation, Thickness margin, Brush foreground)
    {
        Character = character;
        Rotation = rotation;
        Margin = margin;
        DefaultForeground = foreground;
        _foreground = foreground;
    }

    public char Character { get; }

    public double Rotation { get; }

    public Thickness Margin { get; }

    public Brush DefaultForeground { get; }

    public Brush Foreground
    {
        get => _foreground;
        private set => SetProperty(ref _foreground, value);
    }

    public void SetForeground(Brush brush) => Foreground = brush;

    public void ResetForeground() => Foreground = DefaultForeground;
}
