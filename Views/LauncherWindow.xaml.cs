using Pentomino.Algorithms;

using System.Diagnostics;
using System.Linq;
using System.Windows;

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
        PentominoWindow window = new(parallelSolver: true) { Left = this.Left + this.Width + 15, Top = this.Top };
        window.ViewModel.FnCreateMatrix = Algorithms.PentominoMatrix.MakeMatrixFor10x6WithMetadata;
        window.ViewModel.Shapes = PentominoShapeCollections.Make10x6Shapes().ToDictionary(sd => sd.PieceId);

        window.Show();
    }

    private void OnNewCalendarPentominoesWindow(object sender, RoutedEventArgs e)
    {
        if (!GetMonthAndDayDialog.GetMonthAndDate(this, out int month, out int date))
            return;

        PentominoWindow window = new(parallelSolver: true) { Left = this.Left + this.Width + 15, Top = this.Top };
        window.ViewModel.FnCreateMatrix = (shapes) => Algorithms.PentominoCalendarMatrix.MakeMatrixForPentominoCalendarWithMetadata(shapes, month, date);

        window.ViewModel.Shapes = PentominoShapeCollections.MakeCalendarShapes().ToDictionary(sd => sd.PieceId);

        window.ViewModel.BoardWidth = 7;
        window.ViewModel.BoardHeight = 7;

        window.Width = 566;
        window.Height = 560;

        window.Show();
    }

    private void OnNewSudokuWindow(object sender, RoutedEventArgs e)
    {
        Window window = new SudokuWindow { Left = this.Left, Top = this.Top + this.Height + 15 };
        window.Show();
    }

    private void OnNavigateToUri(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        using Process? process = Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}
