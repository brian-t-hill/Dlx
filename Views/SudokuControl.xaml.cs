using Pentomino.ViewModels;

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pentomino.Views;


public partial class SudokuControl : UserControl
{
    public SudokuControl()
    {
        InitializeComponent();
    }


    public SudokuControlViewModel ViewModel => (SudokuControlViewModel) this.DataContext;


    private (int Column, int Row) m_lastMouseDownInBoard = (-1, -1);


    private static (int Column, int Row) GetColumnRowFromBoardObject(object boardObject)
    {
        int column = -1;
        int row = -1;

        if (boardObject is TextBlock tb)
        {
            var dp = VisualTreeHelper.GetParent(tb);

            if (dp is Border b)
            {
                column = Grid.GetColumn(b);
                row = Grid.GetRow(b);
            }
        }

        return (column, row);
    }


    private void OnBoardMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!this.ViewModel.IsEditable)
            return;

        (int column, int row) = GetColumnRowFromBoardObject(e.OriginalSource);

        m_lastMouseDownInBoard = (column, row);
    }


    private void OnBoardMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!this.ViewModel.IsEditable)
            return;

        (int column, int row) = GetColumnRowFromBoardObject(e.OriginalSource);

        if ((column, row) == m_lastMouseDownInBoard)
            this.ViewModel.ApplyInputToBoard(column, row);

        m_lastMouseDownInBoard = (-1, -1);
    }

}
