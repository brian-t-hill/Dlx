using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pentomino.Views;


public partial class LauncherWindow : Window
{
    public LauncherWindow()
    {
        InitializeComponent();
    }


    // I wish it were easier to keep all the built-in window size and position logic, but just change which
    // monitor the window gets created on.  I want these windows to show up on the same monitor as the launcher
    // window.  But I don't feel like recreating all the basic logic and dealing with DPI and all the estorica,
    // so I'm just going to place the Pentomino window to the right of the launcher window and the suduko
    // window will go below it.  Obviously, that will only work well if the launcher window is not on the
    // bottom or the right of the screen.

    private void OnNewPentominoesWindow(object sender, RoutedEventArgs e)
    {
        Window window = new PentominoWindow(parallelSolver: true) { Left = this.Left + this.Width + 15, Top = this.Top };
        window.Show();
    }

    private void OnNewSudokuWindow(object sender, RoutedEventArgs e)
    {
        Window window = new SudokuWindow { Left = this.Left, Top = this.Top + this.Height + 15 };
        window.Show();
    }

    private void OnNavigateToGitHub(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        using Process? process = Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}
