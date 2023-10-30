using Pentomino.Algorithms;
using Pentomino.Helpers;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static Pentomino.Algorithms.Dlx;

namespace Pentomino.ViewModels;


public class DlxMetricsControlViewModel : PropertyChangeNotifier
{
    public DlxMetricsControlViewModel()
    {
    }


    private EventHandler? m_solverElapsedSecondsTimer = null;


    public Dlx.ProgressMetrics ProgressMetrics { get; private set; } = new();


    public void ResetMetrics() => this.ProgressMetrics = new();


    private int m_solvingCount = 0;

    protected int SolvingCount
    {
        get => m_solvingCount;
        set => this.SetProperty(ref m_solvingCount, value);
    }


    private int m_solverElapsedSeconds = 0;

    public int SolverElapsedSeconds
    {
        get => m_solverElapsedSeconds;
        private set => this.SetProperty(ref m_solverElapsedSeconds, value);
    }


    [NotifiesWith(nameof(SolverElapsedSeconds))]
    [NotifiesWith(nameof(SolvingCount))]
    public string SolverElapsedDurationString => TimeSpan.FromSeconds(this.SolverElapsedSeconds).ToString("g");


    [NotifiesWith(nameof(SolverElapsedSeconds))]
    [NotifiesWith(nameof(SolvingCount))]
    public string SolutionCount => this.ProgressMetrics.SolutionCount == 0 ? string.Empty : string.Format(LocalizableStrings.idsSolutionsFoundFormat, this.ProgressMetrics.SolutionCount);


    [NotifiesWith(nameof(SolverElapsedSeconds))]
    [NotifiesWith(nameof(SolvingCount))]
    public string RowsRemovedCount => this.ProgressMetrics.RowsRemoved == 0 ? string.Empty : string.Format(LocalizableStrings.idsRowsRemovedFormat, this.ProgressMetrics.RowsRemoved);


    [NotifiesWith(nameof(SolverElapsedSeconds))]
    [NotifiesWith(nameof(SolvingCount))]
    public string RecursivePassesCount => this.ProgressMetrics.RecursivePasses == 0 ? string.Empty : string.Format(LocalizableStrings.idsRecursivePassesFormat, this.ProgressMetrics.RecursivePasses);


    public void StartSolving()
    {
        if (this.SolvingCount == 0)
        {
            m_solverElapsedSecondsTimer = (o, e) => ++this.SolverElapsedSeconds;

            this.SolverElapsedSeconds = 0;
            PentominoApp.Current.OneSecondTimer += m_solverElapsedSecondsTimer;
        }

        ++this.SolvingCount;
    }


    public void StopSolving()
    {
        --this.SolvingCount;

        if (this.SolvingCount == 0)
        {
            PentominoApp.Current.OneSecondTimer -= m_solverElapsedSecondsTimer;
            m_solverElapsedSecondsTimer = null;
        }
    }


}

