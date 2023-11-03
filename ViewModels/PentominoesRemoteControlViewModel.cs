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


public class PentominoesRemoteControlViewModel : PropertyChangeNotifier
{
    public PentominoesRemoteControlViewModel()
    {
    }


    private int m_numberOfSolutions = 0;

    public int NumberOfSolutions
    {
        get => m_numberOfSolutions;
        set => this.SetProperty(ref m_numberOfSolutions, value);
    }


    private int m_currentSolution = -1;

    public int CurrentSolution
    {
        get => m_currentSolution;
        set => this.SetProperty(ref m_currentSolution, value);
    }


    [NotifiesWithProperty(nameof(NumberOfSolutions))]
    [NotifiesWithProperty(nameof(CurrentSolution))]
    public string SolutionXofY => $"Solution {(this.CurrentSolution >= 0 ? this.CurrentSolution : "?")}/{(this.NumberOfSolutions > 0 ? this.NumberOfSolutions : "?")}";


    private bool m_randomOrder = true;

    public bool RandomOrder
    {
        get => m_randomOrder;
        set => this.SetProperty(ref m_randomOrder, value);
    }

}


