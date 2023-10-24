using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using static Pentomino.ViewModels.SudokuControlViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pentomino.Algorithms;

public static class SudokuMatrix
{
    // row [1,dimension], column [1,dimension], digit [1,dimension]
    private static int GetRowIndex(int dimension, int row, int column, int digit)
    {
        return (row - 1) * dimension * dimension + (column - 1) * dimension + (digit - 1);
    }


    // row [1,dimension], column [1,dimension], digit [1,dimension]
    private static (int Row, int Column, int Digit) GetRowColumnDigitFromRowIndex(int dimension, int rowIndex)
    {
        int digit = (rowIndex % dimension) + 1;
        int column = (rowIndex / dimension) % dimension + 1;
        int row = (rowIndex / dimension) / dimension + 1;

        return (row, column, digit);
    }


    public static bool[/* col */, /* row */] MakePlainMatrixFor9x9()
    {
        bool[,] matrix = new bool[k_NumberOfMatrixColumns_9x9, k_NumberOfMatrixRows_9x9];

        int column = 0;

        // row-column constraints

        for (int y = 1; y <= 9; ++y)
        {
            for (int x = 1; x <= 9; ++x)
            {
                for (int d = 1; d <= 9; ++d)
                {
                    matrix[column, GetRowIndex(9, y, x, d)] = true;
                }

                ++column;
            }
        }

        // row-digit constraints

        for (int y = 1; y <= 9; ++y)
        {
            for (int d = 1; d <= 9; ++d)
            {
                for (int x = 1; x <= 9; ++x)
                {
                    matrix[column, GetRowIndex(9, y, x, d)] = true;
                }

                ++column;
            }
        }

        // column-digit constraints

        for (int x = 1; x <= 9; ++x)
        {
            for (int d = 1; d <= 9; ++d)
            {
                for (int y = 1; y <= 9; ++y)
                {
                    matrix[column, GetRowIndex(9, y, x, d)] = true;
                }

                ++column;
            }
        }

        // box-digit constraints

        for (int boxY = 1; boxY <= 9; boxY += 3)
        {
            for (int boxX = 1; boxX <= 9; boxX += 3)
            {
                for (int d = 1; d <= 9; ++d)
                {
                    for (int deltaY = 0; deltaY < 3; ++deltaY)
                    {
                        for (int deltaX = 0; deltaX < 3; ++deltaX)
                        {
                            matrix[column, GetRowIndex(9, boxY + deltaY, boxX + deltaX, d)] = true;
                        }
                    }

                    ++column;
                }
            }
        }

        return matrix;
    }


    public static bool[/* col */, /* row */] MakeMatrixFor9x9(int[/* row */][/* col */] inputs)
    {
        bool[,] matrix = MakePlainMatrixFor9x9();

        for (int y = 1; y <= 9; ++y)
        {
            for (int x = 1; x <= 9; ++x)
            {
                int inputDigit = inputs[y - 1][x - 1];

                if (inputDigit != 0)
                {
                    // zero out the constraints columns

                    for (int d = 1; d <= 9; ++d)
                    {
                        if (d != inputDigit)
                        {
                            for (int column = 0; column < k_NumberOfMatrixColumns_9x9; ++column)
                            {
                                matrix[column, GetRowIndex(9, y, x, d)] = false;
                            }
                        }
                    }
                }
            }
        }

        return matrix;
    }


    public static int[/* row */][/* col */] MakeOutputsFromSolution(HashSet<int> solution, bool[/* col */, /* row */] matrix)
    {
        int[/* row */][/* col */] output = new int[9][];

        for (int y = 0; y < 9; ++y)
        {
            output[y] = new int[9];
        }

        foreach (int solutionRow in solution)
        {
            (int row, int column, int digit) = GetRowColumnDigitFromRowIndex(9, solutionRow);

            output[row - 1][column - 1] = digit;
        }

        return output;
    }


    private const int k_NumberOfMatrixColumns_9x9 = 324;
    private const int k_NumberOfMatrixRows_9x9 = 729;


}
