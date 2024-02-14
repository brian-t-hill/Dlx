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

namespace Pentomino.Views;

public partial class SudokuWindow : Window
{
    public SudokuWindow()
    {
        this.InitializeComponent();

        this.ViewModel = new();
        this.DataContext = this.ViewModel;
    }


    public SudokuViewModel ViewModel { get; init; }


    public SudokuControlViewModel InputSudokuControlViewModel => this.ViewModel.InputSudokuControlViewModel;

    public SudokuControlViewModel OutputSudokuControlViewModel => this.ViewModel.OutputSudokuControlViewModel;


    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = this.ViewModel.OnClosing();
    }

    private void OnSolutionMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        this.ViewModel.OnPickNextSolution(prev: false);
    }

    private void OnSolutionMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        this.ViewModel.OnPickNextSolution(prev: true);
    }
}
