using Pentomino.Algorithms;
using Pentomino.ViewModels;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pentomino;

public partial class PentominoWindow : Window
{
    public PentominoWindow()
    {
        this.InitializeComponent();

        this.ViewModel = new();
        this.DataContext = this.ViewModel;

        foreach (Shape shape in this.ViewModel.BoardSquares)
        {
            m_grid.Children.Add(shape);
        }

        foreach (Shape shape in this.ViewModel.BoardLines)
        {
            m_grid.Children.Add(shape);
        }

#if false
        bool[,] sampleMatrix = new bool[7, 6]
        {
            { false, false, false, true,  true,  false },
            { false, false, true,  false, false, true  },
            { false, false, false, true,  true,  false },
            { false, true,  false, true,  false, true  },
            { false, true,  false, false, false, false },
            { true,  false, true,  false, false, true  },
            { true,  false, false, false, true,  false },
        };

        List<HashSet<int>> sampleSolutions = Algorithms.Dlx.Solve(sampleMatrix, CancellationToken.None);
#endif
    }


    public PentominoViewModel ViewModel { get; init; }


    private async void OnWindowLoadedAsync(object sender, RoutedEventArgs e)
    {
        await this.ViewModel.OnLoadedAsync(sender, e);
    }


    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = this.ViewModel.OnClosing();
    }

    private void OnWindowMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        this.ViewModel.OnPickANewSolution();
    }
}
