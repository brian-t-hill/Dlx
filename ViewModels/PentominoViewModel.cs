using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Pentomino.Algorithms;
using Pentomino.Helpers;

using static Pentomino.Algorithms.PentominoMatrix;
using static Pentomino.Algorithms.PentominoShapeCollections;

namespace Pentomino.ViewModels;

public class PentominoViewModel : SolvingBaseViewModel
{
    public PentominoViewModel()
    {
        this.Shapes = PentominoShapeCollections.Make10x6Shapes().ToDictionary(sd => sd.PieceId);

        this.ToggleRemoteControlVisibilityCommand = new(() => this.IsRemoteControlVisible = !this.IsRemoteControlVisible);

        this.PentominoesRemoteControlViewModel.OnPropertyChanged(nameof(PentominoesRemoteControlViewModel.OneBasedCurrentSolution), (s, a) => this.CurrentSolution = this.PentominoesRemoteControlViewModel.OneBasedCurrentSolution - 1);
    }


    public PentominoesRemoteControlViewModel PentominoesRemoteControlViewModel { get; } = new();


    private int m_boardWidth = 10;

    public int BoardWidth
    {
        get => m_boardWidth;
        set => this.SetProperty(ref m_boardWidth, value);
    }


    private int m_boardHeight = 6;

    public int BoardHeight
    {
        get => m_boardHeight;
        set => this.SetProperty(ref m_boardHeight, value);
    }


    private Dictionary<int, ShapeData> m_shapes = new();

    public Dictionary<int, ShapeData> Shapes
    {
        get => m_shapes;
        set => this.SetProperty(ref m_shapes, value);
    }


    public event EventHandler? ShapesChanged;


    [CalledWhenPropertyChanges(nameof(Shapes))]
    protected void OnShapesChanged()
    {
        foreach (ShapeData shapeData in this.Shapes.Values)
        {
            if (shapeData.Shape is null)
                continue;

            shapeData.Shape.Stroke = Brushes.Black;
            shapeData.Shape.Fill = shapeData.Brush;
            shapeData.Shape.StrokeThickness = 2;
            shapeData.Shape.HorizontalAlignment = HorizontalAlignment.Left;
            shapeData.Shape.VerticalAlignment = VerticalAlignment.Top;
            shapeData.Shape.RenderTransformOrigin = new(0.5, 0.5);
        }

        var shapesChanged = this.ShapesChanged;
        if (shapesChanged is not null)
            shapesChanged(this, EventArgs.Empty);
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
        this.CanvasScaleX = actualWidth / (100.0 * this.BoardWidth);
        this.CanvasScaleY = actualHeight / (100.0 * this.BoardHeight);
    }


    public bool ParallelSolver { get; set; } = true;

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

        if (m_pentominoMatrixRows is null || m_pentominoMetadataRows is null || this.CurrentSolution < 0 || this.Solutions.Count == 0 || this.CurrentSolution >= this.Solutions.Count)
            return;

        bool randomize = this.PentominoesRemoteControlViewModel.RandomOrder;

        HashSet<int> solution = this.Solutions[randomize ? this.RandomizedSolutionIndexes[this.CurrentSolution] : this.CurrentSolution];

        foreach (int rowIndex in solution)
        {
            // each row is the placement of a single piece.  We can learn about the piece and its placement from our metadata.

            if (m_pentominoMetadataRows[rowIndex].PieceId == (int) CommonPieceIds.None)
                continue;

            ShapeData shapeData = this.Shapes[m_pentominoMetadataRows[rowIndex].PieceId];
            if (shapeData.Shape is null)
                continue;

            shapeData.Shape.RenderTransform = GetRotateAndScaleTransformsFromMetadata(m_pentominoMetadataRows[rowIndex]);

            Canvas.SetLeft(shapeData.Shape, m_pentominoMetadataRows[rowIndex].LeftAdjust * 100);
            Canvas.SetTop(shapeData.Shape, m_pentominoMetadataRows[rowIndex].TopAdjust * 100);
        }
    }


    public override void OnPickNextSolution(bool prev = false)
    {
        if (this.IsSolving || m_pentominoMatrixRows is null || m_pentominoMetadataRows is null || this.Solutions.Count == 0)
            return;

        int currentSolution = this.CurrentSolution;

        if (currentSolution == -1)
            currentSolution = RandomGenerator.Current.Next(this.Solutions.Count);

        int proposedSolution = currentSolution + (prev ? -1 : 1);
        int wrapAround = (prev ? this.Solutions.Count - 1 : 0);

        this.CurrentSolution = (proposedSolution >= 0 && proposedSolution < this.Solutions.Count ? proposedSolution : wrapAround);
    }


    private Func<IEnumerable<PentominoShapeCollections.ShapeData>, (List<bool[]> MatrixRows, List<PlacementMetadata> MetadataRows)>? m_fnCreateMatrix = null;

    public Func<IEnumerable<PentominoShapeCollections.ShapeData>, (List<bool[]> MatrixRows, List<PlacementMetadata> MetadataRows)>? FnCreateMatrix
    {
        get => m_fnCreateMatrix;
        set => this.SetProperty(ref m_fnCreateMatrix, value);
    }


    private List<bool[]>? m_pentominoMatrixRows = null;
    private List<PlacementMetadata>? m_pentominoMetadataRows = null;


    public async Task OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        this.ResetCancellationTokenSource();

        CancellationToken cancelToken = this.SolverCancellationToken;

        this.DlxMetricsControlViewModel.ResetMetrics();

        if (this.FnCreateMatrix is not null)
        {
            (m_pentominoMatrixRows, m_pentominoMetadataRows) = this.FnCreateMatrix(this.Shapes.Values);
            Debug.Assert(m_pentominoMatrixRows.Count == m_pentominoMetadataRows.Count);

            this.IsSolving = true;

            await Task.Run(() =>
            {
                Algorithms.Dlx.Solve(m_pentominoMatrixRows, this.ParallelSolver, cancelToken, this.DlxMetricsControlViewModel.ProgressMetrics);
            });
        }

        this.IsSolving = false;
        this.Solutions = this.DlxMetricsControlViewModel.ProgressMetrics.GetConfirmedSolutions();
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
