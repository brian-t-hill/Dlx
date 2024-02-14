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

public class SudokuViewModel : SolvingBaseViewModel
{
    public SudokuViewModel()
    {
        this.SolveCommand = new(() => !this.IsSolving, async () => await this.SolveAsync());
    }


    public SudokuControlViewModel InputSudokuControlViewModel { get; } = new() { IsEditable = true };

    public SudokuControlViewModel OutputSudokuControlViewModel { get; } = new() { IsEditable = false };


    public override void OnPickNextSolution(bool prev = false)
    {
        if (this.IsSolving || m_sudokuMatrix is null)
            return;

        if (this.Solutions.Count == 0)
        {
            // Clear the previous output.

            int[/* row */][/* col */] blank = new int[9][];

            for (int jj = 0; jj < blank.Length; ++jj)
            {
                blank[jj] = new int[9];
            }

            this.OutputSudokuControlViewModel.ApplyOutputToBoard(blank);

            return;
        }

        if (this.CurrentSolution == -1)
            this.CurrentSolution = RandomGenerator.Current.Next(this.Solutions.Count);

        int proposedSolution = this.CurrentSolution + (prev ? -1 : 1);
        int wrapAround = (prev ? this.Solutions.Count - 1 : 0);

        this.CurrentSolution = (proposedSolution >= 0 && proposedSolution < this.Solutions.Count ? proposedSolution : wrapAround);
        HashSet<int> solution = this.Solutions[this.RandomizedSolutionIndexes[this.CurrentSolution]];

        int[/* row */][/* col */] output = Algorithms.SudokuMatrix.MakeOutputsFromSolution(solution, m_sudokuMatrix, deshuffledRowIndexes: null);
        this.OutputSudokuControlViewModel.ApplyOutputToBoard(output);
    }


    private bool[/* col */, /* row */]? m_sudokuMatrix = null;


    private async Task SolveAsync()
    {
        this.ResetCancellationTokenSource();

        int[/* row */][/* col */] boardInputs = this.InputSudokuControlViewModel.CreateMatrix9x9InputsFromBoard();
        m_sudokuMatrix = Algorithms.SudokuMatrix.MakeMatrix(boardInputs, shuffledRowIndexes: null);

        CancellationToken cancelToken = this.SolverCancellationToken;

        this.DlxMetricsControlViewModel.ResetMetrics();
        this.IsSolving = true;
        List<HashSet<int>>? solutions = null;

        await Task.Run(() =>
        {
            solutions = Algorithms.Dlx.Solve(m_sudokuMatrix, int.MaxValue, cancelToken, this.DlxMetricsControlViewModel.ProgressMetrics);
        });

        this.IsSolving = false;
        this.Solutions = solutions ?? new();
    }




    public bool OnClosing()
    {
        this.ResetCancellationTokenSource();

        return false;
    }


    private Command m_solveCommand = Command.NeverExecute;


    public Command SolveCommand
    {
        get => m_solveCommand;
        private set => this.SetProperty(ref m_solveCommand, value);
    }

}
