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

public partial class PentominoWindow : Window
{
    public PentominoWindow()
    {
        this.InitializeComponent();

        this.ViewModel = new();
        this.DataContext = this.ViewModel;

        foreach (Shape shape in this.ViewModel.Shapes)
        {
            m_canvas.Children.Add(shape);
        }

        m_canvas.SizeChanged += (s, e) => this.ViewModel.OnCanvasSizeChanged(m_canvas.ActualWidth, m_canvas.ActualHeight);
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
        this.ViewModel.OnPickNextSolution(prev: false);
    }

    private void OnWindowMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        this.ViewModel.OnPickNextSolution(prev: true);
    }
}
