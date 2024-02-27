using Pentomino.Algorithms;
using Pentomino.Helpers;
using Pentomino.ViewModels;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
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
    public PentominoWindow(bool parallelSolver)
    {
        this.InitializeComponent();

        this.ViewModel = new() { ParallelSolver = parallelSolver };
        this.DataContext = this.ViewModel;

        foreach (Shape shape in this.ViewModel.Shapes)
        {
            m_canvas.Children.Add(shape);
        }

        m_canvas.SizeChanged += (s, e) => this.ViewModel.OnCanvasSizeChanged(m_canvas.ActualWidth, m_canvas.ActualHeight);

        // REVIEW$:  Whenever I get an IsVisibleChanged event, the control's ActualHeight is zero.
        // I only get a single SizeChanged, so I cache that size and apply it when I get it as well
        // as whenever I get subsequent IsVisibleChanged events.  This works because the size only
        // changes once.  But I worry that something may cause it to change later...

        m_remoteControl.SizeChanged += (s, e) =>
        {
            if (!e.HeightChanged)
                return;

            m_lastKnownRemoteControlHeight = m_remoteControl.ActualHeight;

            if (this.WindowState == WindowState.Normal)
            {
                int sign = (m_remoteControl.IsVisible ? 1 : -1);

                this.Height = this.ActualHeight + sign * -e.PreviousSize.Height + sign * e.NewSize.Height;
            }
        };

        m_remoteControl.IsVisibleChanged += (s, e) =>
        {
            if (this.WindowState == WindowState.Normal)
            {
                int sign = ((bool) e.NewValue ? 1 : -1);

                this.Height = this.ActualHeight + sign * m_lastKnownRemoteControlHeight;
            }
        };
    }


    public PentominoViewModel ViewModel { get; init; }


    public static readonly DependencyProperty RemoteControlTriggerOpacityProperty = DependencyProperty.Register("RemoteControlTriggerOpacity", typeof(double), typeof(PentominoWindow), new UIPropertyMetadata(0.0));

    public double RemoteControlTriggerOpacity
    {
        get => (double) GetValue(RemoteControlTriggerOpacityProperty);
        set => SetValue(RemoteControlTriggerOpacityProperty, value);
    }


    private double m_lastKnownRemoteControlHeight = 0.0;

    private async void OnWindowLoadedAsync(object sender, RoutedEventArgs e)
    {
        await this.ViewModel.OnLoadedAsync(sender, e);
    }


    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = this.ViewModel.OnClosing();
    }

    private void OnCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        this.ViewModel.OnPickNextSolution(prev: false);
    }

    private void OnCanvasMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        this.ViewModel.OnPickNextSolution(prev: true);
    }

    private void OnCanvasPreviewMouseMove(object sender, MouseEventArgs e)
    {
        // Set the opacity of the remote control trigger based on the distance of the mouse to the trigger

        Point ptMouse = e.GetPosition(m_canvasGrid);

        GeneralTransform transform = m_remoteControlTrigger.TransformToAncestor(m_canvasGrid);
        Point ptTrigger = transform.Transform(new Point(0, 0));

        (double x, double y) = (ptMouse.X - ptTrigger.X, ptMouse.Y - ptTrigger.Y);
        double distance = Math.Sqrt((x * x) + (y * y));

        double minimum = 40.0;
        double maximum = Math.Max(minimum, Math.Min(m_canvasGrid.ActualHeight, m_canvasGrid.ActualWidth) * 2.0 / 3.0);
        double range = Math.Max(double.Epsilon, maximum - minimum);

        if (distance < minimum)
            this.RemoteControlTriggerOpacity = 1.0;
        else if (distance > maximum)
            this.RemoteControlTriggerOpacity = 0.0;
        else
            this.RemoteControlTriggerOpacity = 1.0 - ((distance - minimum) / range);
    }
}
