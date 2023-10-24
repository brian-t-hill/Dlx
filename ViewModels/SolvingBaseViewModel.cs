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

public abstract class SolvingBaseViewModel : PropertyChangeNotifier
{
    public SolvingBaseViewModel()
    {
    }


    private EventHandler? m_solverElapsedSecondsTimer = null;


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


    private int m_solverElapsedSeconds = 0;

    public int SolverElapsedSeconds
    {
        get => m_solverElapsedSeconds;
        private set => this.SetProperty(ref m_solverElapsedSeconds, value);
    }


    [NotifiesWith(nameof(SolverElapsedSeconds))]
    [NotifiesWith(nameof(Solutions))]
    public string SolverElapsedDurationString => TimeSpan.FromSeconds(this.SolverElapsedSeconds).ToString("g");


    [NotifiesWith(nameof(SolverElapsedSeconds))]
    [NotifiesWith(nameof(Solutions))]
    public string SolutionCount => m_progressCount.Count == 0 ? string.Empty : $"Solutions found: {m_progressCount.Count:n0}";


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

    protected Dlx.ProgressCount m_progressCount = new();


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
