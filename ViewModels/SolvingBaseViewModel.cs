using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
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

public abstract class SolvingBaseViewModel : PropertyChangeNotifier
{
    public SolvingBaseViewModel()
    {
    }


    public DlxMetricsControlViewModel DlxMetricsControlViewModel { get; } = new();


    private bool m_isSolving = false;

    public bool IsSolving
    {
        get => m_isSolving;
        protected set => this.SetProperty(ref m_isSolving, value);
    }



    [CalledWhenPropertyChanges(nameof(IsSolving))]
    protected void OnIsSolvingChanged()
    {
        if (this.IsSolving)
            this.DlxMetricsControlViewModel.StartSolving();
        else
            this.DlxMetricsControlViewModel.StopSolving();
    }


    private List<HashSet<int>> m_solutions = new();

    public List<HashSet<int>> Solutions
    {
        get => m_solutions;
        set => this.SetProperty(ref m_solutions, value);
    }


    private List<int> m_randomizedSolutionIndexes = new();

    public List<int> RandomizedSolutionIndexes
    {
        get => m_randomizedSolutionIndexes;
        set => this.SetProperty(ref m_randomizedSolutionIndexes, value);
    }


    private int m_currentSolution = -1;

    protected int CurrentSolution
    {
        get => m_currentSolution;
        set => this.SetProperty(ref m_currentSolution, value);
    }


    [CalledWhenPropertyChanges(nameof(Solutions))]
    protected void OnSolutionsChanged()
    {
        List<int> randomizedSolutionIndexes = new(Enumerable.Range(0, this.Solutions.Count));
        randomizedSolutionIndexes.Shuffle();
        this.RandomizedSolutionIndexes = randomizedSolutionIndexes;

        this.OnPickNextSolution(prev: false);
    }


    public abstract void OnPickNextSolution(bool prev = false);


    private CancellationTokenSource m_solverCts = new();

    public CancellationToken SolverCancellationToken => m_solverCts.Token;

    protected void ResetCancellationTokenSource()
    {
        m_solverCts.Cancel();
        m_solverCts = new();

        this.CancelCommand = new(() => this.ResetCancellationTokenSource());
    }


    private Command m_cancelCommand = Command.NeverExecute;


    public Command CancelCommand
    {
        get => m_cancelCommand;
        private set => this.SetProperty(ref m_cancelCommand, value);
    }

}
