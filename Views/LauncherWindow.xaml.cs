using System;
using System.Collections.Generic;
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

    private void OnNewPentominoesWindow(object sender, RoutedEventArgs e)
    {
        Window window = new PentominoWindow();
        window.Show();
    }

    private void OnNewSudokuWindow(object sender, RoutedEventArgs e)
    {
        Window window = new SudokuWindow();
        window.Show();
    }
}
