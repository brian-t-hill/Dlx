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


public class SudokuControlViewModel : PropertyChangeNotifier
{
    public class IntViewModel : PropertyChangeNotifier
    {
        private int m_value = 0;

        public int Value
        {
            get => m_value;
            set => this.SetProperty(ref m_value, value);
        }


        [NotifiesWithProperty(nameof(Value))]
        public string ValueAsString => (this.Value == 0 ? " " : this.Value.ToString());
    }


    // Normally, I like multi-dimensional arrays to be in x,y order, but since this is jagged,
    // I think I prefer the rows to be in a contiguous slices, rather than the columns. As a
    // result, this array is in y,x order.
    //
    public IntViewModel[/* row */][/* col */] Board { get; } = new IntViewModel[9][];

    public SudokuControlViewModel()
    {
        for (int y = 0; y < this.Board.Length; ++y)
        {
            this.Board[y] = new IntViewModel[9];

            for (int x = 0; x < this.Board[y].Length; ++x)
            {
                this.Board[y][x] = new() { Value = 0 };
            }
        }

        this.RandomizeCommand = new(() => this.IsEditable, () =>
        {
            int[/* row */][/* col */] inputs = SudokuMatrix.GenerateRandomOpeningInputs(9);

            this.ApplyOutputToBoard(inputs);
        });
    }


    private bool m_isEditable = false;

    public bool IsEditable
    {
        get => m_isEditable;
        set => this.SetProperty(ref m_isEditable, value);
    }


    private int m_inputValue = 1;

    public int InputValue
    {
        get => m_inputValue;
        set => this.SetProperty(ref m_inputValue, value);
    }

    public void ApplyInputToBoard(int column, int row)
    {
        if (column >= 0 && row >= 0)
            this.Board[row][column].Value = this.InputValue;
    }


    public int[/* row */][/* col */] CreateMatrix9x9InputsFromBoard()
    {
        int[/* row */][/* col */] inputs = new int[9][];

        for (int y = 0; y < 9; ++y)
        {
            inputs[y] = new int[9];

            for (int x = 0; x < 9; ++x)
            {
                inputs[y][x] = this.Board[y][x].Value;
            }
        }

        return inputs;
    }


    public void ApplyOutputToBoard(int[/* row */][/* col */] output)
    {
        for (int row = 0; row < 9; ++row)
        {
            for (int column = 0; column < 9; ++column)
            {
                this.Board[row][column].Value = output[row][column];
            }
        }
    }


    public Command RandomizeCommand { get; }

}

