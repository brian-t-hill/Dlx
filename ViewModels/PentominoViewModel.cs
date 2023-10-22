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

using Pentomino.Helpers;

using static Pentomino.Algorithms.PentominoMatrix;

namespace Pentomino.ViewModels;

public class PentominoViewModel : PropertyChangeNotifier
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
    private const int k_Empty_Index = 12;
    private const int k_Border_Index = 13;
    private const int k_NoBorder_Index = 14;

    public PentominoViewModel()
    {
        this.Shapes = new Shape[]
        {
            new Polygon()
            {
                // . F F
                // F F .
                // . F .

                Points = new() { new(100, 0), new(300, 0), new(300, 100), new(200, 100), new(200, 300), new(100, 300), new(100, 200), new(0, 200), new(0, 100), new(100, 100) },
            },

            new Polygon()
            {
                // . . . L
                // L L L L

                Points = new() { new(300, 0), new(400, 0), new(400, 200), new(00, 200), new(0, 100), new(300, 100) },
            },

            new Polygon()
            {
                // I I I I I

                Points = new() { new(0, 0), new(500, 0), new(500, 100), new(0, 100) },
            },

            new Polygon()
            {
                // P .
                // P P
                // P P

                Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 300), new(0, 300) },
            },

            new Polygon()
            {
                // . . S S
                // S S S .

                Points = new() { new(200, 0), new(400, 0), new(400, 100), new(300, 100), new(300, 200), new(0, 200), new(0, 100), new(200, 100) },
            },

            new Polygon()
            {
                // T T T
                // . T .
                // . T .

                Points = new() { new(0, 0), new(300, 0), new(300, 100), new(200, 100), new(200, 300), new(100, 300), new(100, 100), new(0, 100) },
            },

            new Polygon()
            {
                // U . U
                // U U U

                Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 0), new(300, 0), new(300, 200), new(0, 200) },
            },

            new Polygon()
            {
                // V . .
                // V . .
                // V V V

                Points = new() { new(0, 0), new(100, 0), new(100, 200), new(300, 200), new(300, 300), new(0, 300) },
            },

            new Polygon()
            {
                // W . .
                // W W .
                // . W W

                Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 200), new(300, 200), new(300, 300), new(100, 300), new(100, 200), new(0, 200) },
            },

            new Polygon()
            {
                // . X .
                // X X X
                // . X .

                Points = new() { new(100, 0), new(200, 0), new(200, 100), new(300, 100), new(300, 200), new(200, 200), new(200, 300), new(100, 300), new(100, 200), new(0, 200), new(0, 100), new(100, 100) },
            },

            new Polygon()
            {
                // . . Y .
                // Y Y Y Y

                Points = new() { new(200, 0), new(300, 0), new(300, 100), new(400, 100), new(400, 200), new(0, 200), new(0, 100), new(200, 100) },
            },

            new Polygon()
            {
                // Z Z .
                // . Z .
                // . Z Z

                Points = new() { new(0, 0), new(200, 0), new(200, 200), new(300, 200), new(300, 300), new(100, 300), new(100, 100), new(0, 100) },
            },
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

        TransformGroup transformGroup = new();
        transformGroup.Children.Add(new RotateTransform() { Angle = 180 });
        transformGroup.Children.Add(new TranslateTransform() { X = -10, Y = -10 });

        this.Shapes[k_F_Index].RenderTransform = transformGroup;

        transformGroup = new();
        transformGroup.Children.Add(new RotateTransform() { Angle = 90 });
        transformGroup.Children.Add(new ScaleTransform() { ScaleY = -1 });
        transformGroup.Children.Add(new TranslateTransform() { X = -20, Y = -10 });

        this.Shapes[k_L_Index].RenderTransform = transformGroup;

        this.Shapes[k_I_Index].RenderTransform = new TranslateTransform() { X = 30, Y = -20 };

        this.Shapes[k_P_Index].RenderTransform = new TranslateTransform() { X = 40, Y = 10 };

        this.Shapes[k_S_Index].RenderTransform = new TranslateTransform() { X = 0, Y = 30 };

        transformGroup = new();
        transformGroup.Children.Add(new ScaleTransform() { ScaleY = -1 });
        transformGroup.Children.Add(new TranslateTransform() { X = 20, Y = -20 });

        this.Shapes[k_T_Index].RenderTransform = transformGroup;

        transformGroup = new();
        transformGroup.Children.Add(new ScaleTransform() { ScaleY = -1 });
        transformGroup.Children.Add(new TranslateTransform() { X = 60, Y = 10 });

        this.Shapes[k_U_Index].RenderTransform = transformGroup;

        transformGroup = new();
        transformGroup.Children.Add(new ScaleTransform() { ScaleX = -1 });
        transformGroup.Children.Add(new TranslateTransform() { X = 60, Y = -10 });

        this.Shapes[k_V_Index].RenderTransform = transformGroup;

        transformGroup = new();
        transformGroup.Children.Add(new ScaleTransform() { ScaleY = -1 });
        transformGroup.Children.Add(new TranslateTransform() { X = 20, Y = 0 });

        this.Shapes[k_W_Index].RenderTransform = transformGroup;

        this.Shapes[k_X_Index].RenderTransform = new TranslateTransform() { X = 0, Y = -10 };

        this.Shapes[k_Y_Index].RenderTransform = new TranslateTransform() { X = 60, Y = 30 };

        transformGroup = new();
        transformGroup.Children.Add(new RotateTransform() { Angle = 90 });
        transformGroup.Children.Add(new TranslateTransform() { X = 50, Y = -10 });

        this.Shapes[k_Z_Index].RenderTransform = transformGroup;
    }


    public EventHandler? m_solverElapsedSecondsTimer = null;


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

        /* _ */ new SolidColorBrush(Colors.Ivory),  // k_Empty_Index

        /* | */ new SolidColorBrush(Colors.Black),  // k_Border_Index
        /*   */ new SolidColorBrush(Colors.Transparent),  // k_NoBorder_Index
    };


    public Shape[] Shapes { get; private set; }


    private bool m_isSolving = false;

    public bool IsSolving
    {
        get => m_isSolving;
        private set => this.SetProperty(ref m_isSolving, value);
    }


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


    [CalledWhenPropertyChanges(nameof(IsSolving))]
    private void OnIsSolvingChanged()
    {
        if (this.IsSolving)
        {
            m_solverElapsedSecondsTimer = (o, e) => ++this.SolverElapsedSeconds;

            this.SolverElapsedSeconds = 0;
            PentominoApp.Current.OneSecondTimer += m_solverElapsedSecondsTimer;
        }
        else
        {
            PentominoApp.Current.OneSecondTimer -= m_solverElapsedSecondsTimer;
            m_solverElapsedSecondsTimer = null;
        }
    }


    private List<HashSet<int>> m_pentominoSolutions = new();

    public List<HashSet<int>> PentominoSolutions
    {
        get => m_pentominoSolutions;
        set => this.SetProperty(ref m_pentominoSolutions, value);
    }


    private int m_solverElapsedSeconds = 0;

    public int SolverElapsedSeconds
    {
        get => m_solverElapsedSeconds;
        private set => this.SetProperty(ref m_solverElapsedSeconds, value);
    }


    [NotifiesWith(nameof(SolverElapsedSeconds))]
    public string SolverElapsedDurationString => TimeSpan.FromSeconds(this.SolverElapsedSeconds).ToString("g");


    [CalledWhenPropertyChanges(nameof(PentominoSolutions))]
    private void OnSolutionsChanged()
    {
        this.OnPickANewSolution();
    }


    private static Transform GetTransformFromMetadata(PlacementMetadata metadata)
    {
        RotateTransform? rotation = (metadata.Angle != 0 ? new RotateTransform { Angle = metadata.Angle } : null);
        TranslateTransform? translation = null; // (metadata.TranslateY != 0 || metadata.TranslateY != 0 ? new TranslateTransform { X = metadata.TranslateX * 100, Y = metadata.TranslateY * 100} : null);
        ScaleTransform? scale = (metadata.ScaleX != 1 || metadata.ScaleY != 1 ? new ScaleTransform { ScaleX = metadata.ScaleX, ScaleY = metadata.ScaleY} : null);

        int count = (rotation is null ? 0 : 1) + (translation is null ? 0 : 1) + (scale is null ? 0 : 1);

        if (count == 0)
            return Transform.Identity;

        if (count == 1)
            return (Transform?) rotation ?? (Transform?) translation ?? scale!;

        TransformGroup transformGroup = new();

        if (rotation is not null)
            transformGroup.Children.Add(rotation);

        if (translation is not null)
            transformGroup.Children.Add(translation);

        if (scale is not null)
            transformGroup.Children.Add(scale);

        return transformGroup;
    }


    public void OnPickANewSolution()
    {
        if (this.IsSolving || m_pentominoMatrix is null || m_pentominoPlacementMetadata is null)
            return;

        int solutionIndex = (new Random()).Next(this.PentominoSolutions.Count);
        HashSet<int> solution = this.PentominoSolutions[solutionIndex];

        char[,] board = Algorithms.PentominoMatrix.ComposeBoard(10, 6, solution, this.m_pentominoMatrix);
        board.GetType();

        foreach (int rowIndex in solution)
        {
            // each row is the placement of a single piece.  We can learn about the piece and its placement from our metadata.

            int piece = m_pentominoPlacementMetadata[rowIndex].Piece;
            Shape shape = this.Shapes[piece];

            shape.RenderTransform = GetTransformFromMetadata(m_pentominoPlacementMetadata[rowIndex]);

            Canvas.SetLeft(shape, m_pentominoPlacementMetadata[rowIndex].LeftAdjust * 100);
            Canvas.SetTop(shape, m_pentominoPlacementMetadata[rowIndex].TopAdjust * 100);
        }
    }


    private bool[,]? m_pentominoMatrix = null;
    private PlacementMetadata[]? m_pentominoPlacementMetadata = null;

    CancellationTokenSource m_pentominoSolverCts = new CancellationTokenSource();

    public async Task OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        (m_pentominoMatrix, m_pentominoPlacementMetadata) = Algorithms.PentominoMatrix.MakeMatrixFor10x6WithMetadata();

        CancellationToken cancelToken = m_pentominoSolverCts.Token;

        this.IsSolving = true;
        List<HashSet<int>>? solutions = null;

        await Task.Run(() =>
        {
            solutions = Algorithms.Dlx.Solve(m_pentominoMatrix, cancelToken);
        });

        this.IsSolving = false;
        this.PentominoSolutions = solutions ?? new();
    }

    public bool OnClosing()
    {
        m_pentominoSolverCts.Cancel();
        m_pentominoSolverCts = new();

        return false;
    }

}
