using Pentomino.Algorithms;
using Pentomino.Helpers;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static Pentomino.Algorithms.Dlx;

namespace Pentomino.ViewModels;


public class PentominoesRemoteControlViewModel : PropertyChangeNotifier
{
    public PentominoesRemoteControlViewModel()
    {
        this.PreviousCommand =  new(() => this.OneBasedCurrentSolution > 1, () => { --this.OneBasedCurrentSolution; });
        this.NextCommand =  new(() => this.OneBasedCurrentSolution < this.NumberOfSolutions, () => { ++this.OneBasedCurrentSolution; });
    }


    private int m_numberOfSolutions = 0;

    public int NumberOfSolutions
    {
        get => m_numberOfSolutions;
        set => this.SetProperty(ref m_numberOfSolutions, value);
    }


    private int m_oneBasedcurrentSolution = 0;

    public int OneBasedCurrentSolution
    {
        get => m_oneBasedcurrentSolution;
        set => this.SetProperty(ref m_oneBasedcurrentSolution, value);
    }


    [NotifiesWithProperty(nameof(NumberOfSolutions))]
    [NotifiesWithProperty(nameof(OneBasedCurrentSolution))]
    public string SolutionXofY => $"Solution {(this.OneBasedCurrentSolution > 0 ? this.OneBasedCurrentSolution : "?")}/{(this.NumberOfSolutions > 0 ? this.NumberOfSolutions : "?")}";


    private bool m_randomOrder = true;

    public bool RandomOrder
    {
        get => m_randomOrder;
        set => this.SetProperty(ref m_randomOrder, value);
    }


    [CanExecuteChangesWithProperty(nameof(OneBasedCurrentSolution))]
    public Command PreviousCommand { get; }


    [CanExecuteChangesWithProperty(nameof(OneBasedCurrentSolution))]
    [CanExecuteChangesWithProperty(nameof(NumberOfSolutions))]
    public Command NextCommand { get; }
}


