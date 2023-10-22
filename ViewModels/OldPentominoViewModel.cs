using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Pentomino.Helpers;

namespace Pentomino.ViewModels;

public class OldPentominoViewModel : PropertyChangeNotifier
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

    public OldPentominoViewModel()
    {
        this.BoardSquares = new Shape[60];

        for (int jj = 0; jj < 60; ++jj)
        {
            int column = jj % 10 * 2;
            int row = jj / 10 * 2;

            this.BoardSquares[jj] = new Rectangle();

            Grid.SetColumn(this.BoardSquares[jj], column);
            Grid.SetRow(this.BoardSquares[jj], row);

            this.BoardSquares[jj].Fill = Brushes[k_Empty_Index];
        }

        this.BoardLines = new Shape[104];

        // vertical lines

        for (int jj = 0; jj < 54; ++jj)
        {
            int column = jj % 9 * 2 + 1;
            int row = jj / 9 * 2;

            this.BoardLines[jj] = new Rectangle();

            Grid.SetColumn(this.BoardLines[jj], column);
            Grid.SetRow(this.BoardLines[jj], row);

            this.BoardLines[jj].Fill = Brushes[k_NoBorder_Index];
        }

        // horizontal lines

        for (int jj = 0; jj < 50; ++jj)
        {
            int index = jj + 54;

            int column = jj % 10 * 2;
            int row = jj / 10 * 2 + 1;

            this.BoardLines[index] = new Rectangle();

            Grid.SetColumn(this.BoardLines[index], column);
            Grid.SetRow(this.BoardLines[index], row);

            this.BoardLines[jj].Fill = Brushes[k_NoBorder_Index];
        }
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


    public Shape[] BoardSquares { get; private set; }

    public Shape[] BoardLines { get; private set; }


    private bool m_isSolving = false;

    public bool IsSolving
    {
        get => m_isSolving;
        private set => this.SetProperty(ref m_isSolving, value);
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


    private readonly int[] m_fragments = new int[60];  // index is which spot on board (0-59). Value is which piece (FLIPSTUVWXYZ + empty).


    private int GetShapeIndexFromSolutionRow(int rowIndex)
    {
        if (m_pentominoMatrix is null)
            return 13;

        for (int jj = 0; jj < 12; ++jj)
        {
            if (m_pentominoMatrix[jj, rowIndex])
                return jj;
        }

        return 13;
    }


    private IEnumerable<int> GetBoardPositionsFromSolutionRow(int rowIndex)
    {
        if (m_pentominoMatrix is null)
            yield break;

        for (int jj = 0; jj < 60; ++jj)
        {
            if (m_pentominoMatrix[jj + 12, rowIndex])
                yield return jj;
        }
    }


    [CalledWhenPropertyChanges(nameof(PentominoSolutions))]
    private void OnSolutionsChanged()
    {
        this.OnPickANewSolution();
    }


    public void OnPickANewSolution()
    {
        if (this.IsSolving || m_pentominoMatrix is null)
            return;

        foreach (int rowIndex in this.PentominoSolutions[(new Random()).Next(this.PentominoSolutions.Count)])
        {
            int shape = GetShapeIndexFromSolutionRow(rowIndex);

            foreach (var boardPosition in this.GetBoardPositionsFromSolutionRow(rowIndex))
            {
                m_fragments[boardPosition] = shape;
            }
        }

        this.OnFragmentsUpdated();
    }


    private void OnFragmentsUpdated()
    {
        // Color the board squares

        for (int jj = 0; jj < 60; ++jj)
        {
            this.BoardSquares[jj].Fill = Brushes[m_fragments[jj]];
        }


        // Color the lines

        for (int y = 0; y < 6; ++y)
        {
            for (int x = 0; x < 10; ++x)
            {
                int boardIndex = y * 10 + x;
                int horizontalNeighborIndex = boardIndex + 1;
                int verticalNeighborIndex = boardIndex + 10;

                int verticalLineIndex = y * 9 + x;
                int horizontalLineIndex = 54 + y * 10 + x;

                if (x < 9)
                    this.BoardLines[verticalLineIndex].Fill = Brushes[(m_fragments[boardIndex] == m_fragments[horizontalNeighborIndex]) ? m_fragments[boardIndex] : k_Border_Index];

                if (y < 5)
                    this.BoardLines[horizontalLineIndex].Fill = Brushes[(m_fragments[boardIndex] == m_fragments[verticalNeighborIndex]) ? m_fragments[boardIndex] : k_Border_Index];
            }
        }
    }


    bool[,]? m_pentominoMatrix = null;


    CancellationTokenSource m_pentominoSolverCts = new CancellationTokenSource();

    public async Task OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        m_pentominoMatrix = Algorithms.PentominoMatrix.MakeMatrixFor10x6();

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
