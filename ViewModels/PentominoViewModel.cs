using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

using Pentomino.Algorithms;
using Pentomino.Helpers;

using static Pentomino.Algorithms.Dlx;
using static Pentomino.Algorithms.PentominoMatrix;

namespace Pentomino.ViewModels;

public class PentominoViewModel : SolvingBaseViewModel
{
    private const int k_F_Index = 0;
    private const int k_L_Index = 1;
    private const int k_I_Index = 2;
    private const int k_P_Index = 3;
    private const int k_S_Index = 4;
    private const int k_T_Index = 5;
    private const int k_U_Index = 6;
    private const int k_V_Index = 7;
    private const int k_W_Index = 8;
    private const int k_X_Index = 9;
    private const int k_Y_Index = 10;
    private const int k_Z_Index = 11;
    private const int k_Border_Index = 12;

    private static readonly Brush[] Brushes = new Brush[]
    {
        /* F */ new SolidColorBrush(Colors.Peru),
        /* L */ new SolidColorBrush(Colors.Red),
        /* I */ new SolidColorBrush(Colors.LightSteelBlue),
        /* P */ new SolidColorBrush(Colors.Yellow),
        /* S */ new SolidColorBrush(Colors.DarkGray),
        /* T */ new SolidColorBrush(Colors.Firebrick),
        /* U */ new SolidColorBrush(Colors.DarkOrchid),
        /* V */ new SolidColorBrush(Colors.RoyalBlue),
        /* W */ new SolidColorBrush(Colors.Lime),
        /* X */ new SolidColorBrush(Colors.NavajoWhite),
        /* Y */ new SolidColorBrush(Colors.Green),
        /* Z */ new SolidColorBrush(Colors.Aquamarine),

        /* | */ new SolidColorBrush(Colors.Black),  // k_Border_Index
    };


    public PentominoViewModel()
    {
        this.Shapes = new Shape[]
        {
            // . F F
            // F F .
            // . F .
            new Polygon() { Points = new() { new(100, 0), new(300, 0), new(300, 100), new(200, 100), new(200, 300), new(100, 300), new(100, 200), new(0, 200), new(0, 100), new(100, 100) } },

            // L .
            // L .
            // L .
            // L L
            new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 300), new(200, 300), new(200, 400), new(0, 400) } },

            // I
            // I
            // I
            // I
            // I
            new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 500), new(0, 500) } },

            // P P
            // P P
            // P .
            new Polygon() { Points = new() { new(0, 0), new(200, 0), new(200, 200), new(100, 200), new(100, 300), new(0, 300) } },

            // S
            // S S
            // . S
            // . S
            new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 400), new(100, 400), new(100, 200), new(0, 200) } },

            // T T T
            // . T .
            // . T .
            new Polygon() { Points = new() { new(0, 0), new(300, 0), new(300, 100), new(200, 100), new(200, 300), new(100, 300), new(100, 100), new(0, 100) } },

            // U . U
            // U U U
            new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 0), new(300, 0), new(300, 200), new(0, 200) } },

            // V . .
            // V . .
            // V V V
            new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 200), new(300, 200), new(300, 300), new(0, 300) } },

            // W . .
            // W W .
            // . W W
            new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 200), new(300, 200), new(300, 300), new(100, 300), new(100, 200), new(0, 200) } },

            // . X .
            // X X X
            // . X .
            new Polygon() { Points = new() { new(100, 0), new(200, 0), new(200, 100), new(300, 100), new(300, 200), new(200, 200), new(200, 300), new(100, 300), new(100, 200), new(0, 200), new(0, 100), new(100, 100) } },

            // . . Y .
            // Y Y Y Y
            new Polygon() { Points = new() { new(200, 0), new(300, 0), new(300, 100), new(400, 100), new(400, 200), new(0, 200), new(0, 100), new(200, 100) } },

            // Z Z .
            // . Z .
            // . Z Z
            new Polygon() { Points = new() { new(0, 0), new(200, 0), new(200, 200), new(300, 200), new(300, 300), new(100, 300), new(100, 100), new(0, 100) } },
        };

        for (int jj = k_F_Index; jj <= k_Z_Index; ++jj)
        {
            this.Shapes[jj].Stroke = Brushes[k_Border_Index];
            this.Shapes[jj].Fill = Brushes[jj];
            this.Shapes[jj].StrokeThickness = 2;
            this.Shapes[jj].HorizontalAlignment = HorizontalAlignment.Left;
            this.Shapes[jj].VerticalAlignment = VerticalAlignment.Top;
            this.Shapes[jj].RenderTransformOrigin = new(0.5, 0.5);

            Canvas.SetLeft(this.Shapes[jj], 0);
            Canvas.SetTop(this.Shapes[jj], 0);
        }

        this.ToggleRemoteControlVisibilityCommand = new(() => this.IsRemoteControlVisible = !this.IsRemoteControlVisible);

        this.PentominoesRemoteControlViewModel.OnPropertyChanged(nameof(PentominoesRemoteControlViewModel.OneBasedCurrentSolution), (s, a) => this.CurrentSolution = this.PentominoesRemoteControlViewModel.OneBasedCurrentSolution - 1);
    }


    public PentominoesRemoteControlViewModel PentominoesRemoteControlViewModel { get; } = new();


    public Shape[] Shapes { get; private set; }


    private double m_canvasScaleX = 1.0;

    public double CanvasScaleX
    {
        get => m_canvasScaleX;
        set => this.SetProperty(ref m_canvasScaleX, value);
    }


    private double m_canvasScaleY = 1.0;

    public double CanvasScaleY
    {
        get => m_canvasScaleY;
        set => this.SetProperty(ref m_canvasScaleY, value);
    }


    public void OnCanvasSizeChanged(double actualWidth, double actualHeight)
    {
        this.CanvasScaleX = actualWidth / 1000.0;
        this.CanvasScaleY = actualHeight / 600.0;
    }


    private static Transform GetRotateAndScaleTransformsFromMetadata(PlacementMetadata metadata)
    {
        RotateTransform? rotation = (metadata.Angle != 0 ? new RotateTransform { Angle = metadata.Angle } : null);
        ScaleTransform? scale = (metadata.Flip ? new ScaleTransform { ScaleX = -1.0, ScaleY = 1.0 } : null);

        int count = (rotation is null ? 0 : 1) + (scale is null ? 0 : 1);

        if (count == 0)
            return Transform.Identity;

        if (count == 1)
            return (Transform?) rotation ?? scale!;

        TransformGroup transformGroup = new();

        if (scale is not null)
            transformGroup.Children.Add(scale);

        if (rotation is not null)
            transformGroup.Children.Add(rotation);

        return transformGroup;
    }


    [CalledWhenPropertyChanges(nameof(Solutions))]
    protected void OnBaseSolutionsChanged()
    {
        this.PentominoesRemoteControlViewModel.NumberOfSolutions = this.Solutions.Count;
    }


    [CalledWhenPropertyChanges(nameof(CurrentSolution))]
    protected void OnBaseCurrentSolutionChanged()
    {
        this.PentominoesRemoteControlViewModel.OneBasedCurrentSolution = this.CurrentSolution + 1;

        if (m_pentominoMatrix is null || m_pentominoPlacementMetadata is null || this.CurrentSolution < 0 || this.Solutions.Count == 0 || this.CurrentSolution >= this.Solutions.Count)
            return;

        bool randomize = this.PentominoesRemoteControlViewModel.RandomOrder;

        HashSet<int> solution = this.Solutions[randomize ? this.RandomizedSolutionIndexes[this.CurrentSolution] : this.CurrentSolution];

        foreach (int rowIndex in solution)
        {
            // each row is the placement of a single piece.  We can learn about the piece and its placement from our metadata.

            int piece = m_pentominoPlacementMetadata[rowIndex].Piece;
            Shape shape = this.Shapes[piece];

            shape.RenderTransform = GetRotateAndScaleTransformsFromMetadata(m_pentominoPlacementMetadata[rowIndex]);

            Canvas.SetLeft(shape, m_pentominoPlacementMetadata[rowIndex].LeftAdjust * 100);
            Canvas.SetTop(shape, m_pentominoPlacementMetadata[rowIndex].TopAdjust * 100);
        }
    }


    public override void OnPickNextSolution(bool prev = false)
    {
        if (this.IsSolving || m_pentominoMatrix is null || m_pentominoPlacementMetadata is null || this.Solutions.Count == 0)
            return;

        int currentSolution = this.CurrentSolution;

        if (currentSolution == -1)
            currentSolution = RandomGenerator.Current.Next(this.Solutions.Count);

        int proposedSolution = currentSolution + (prev ? -1 : 1);
        int wrapAround = (prev ? this.Solutions.Count - 1 : 0);

        this.CurrentSolution = (proposedSolution >= 0 && proposedSolution < this.Solutions.Count ? proposedSolution : wrapAround);
    }


    private bool[,]? m_pentominoMatrix = null;
    private PlacementMetadata[]? m_pentominoPlacementMetadata = null;

    public async Task OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        this.ResetCancellationTokenSource();

        (m_pentominoMatrix, m_pentominoPlacementMetadata) = Algorithms.PentominoMatrix.MakeMatrixFor10x6WithMetadata();

        CancellationToken cancelToken = this.SolverCancellationToken;

        this.DlxMetricsControlViewModel.ResetMetrics();
        this.IsSolving = true;
        List<HashSet<int>>? solutions = null;

        await Task.Run(() =>
        {
            solutions = Algorithms.Dlx.Solve(m_pentominoMatrix, cancelToken, this.DlxMetricsControlViewModel.ProgressMetrics);
        });

        this.IsSolving = false;
        this.Solutions = solutions ?? new();
    }

    public bool OnClosing()
    {
        this.ResetCancellationTokenSource();

        return false;
    }


    private bool m_isRemoteControlVisible = false;

    public bool IsRemoteControlVisible
    {
        get => m_isRemoteControlVisible;
        set => this.SetProperty(ref m_isRemoteControlVisible, value);
    }


    [NotifiesWithProperty(nameof(IsRemoteControlVisible))]
    public string TriggerSymbol => this.IsRemoteControlVisible ? LocalizableStrings.idsUpArrow : LocalizableStrings.idsDownArrow;


    private Command m_toggleRemoteControlVisibilityCommand = Command.NeverExecute;

    public Command ToggleRemoteControlVisibilityCommand
    {
        get => m_toggleRemoteControlVisibilityCommand;
        private set => this.SetProperty(ref m_toggleRemoteControlVisibilityCommand, value);
    }

}
