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

// Sudoku is defined by the number of digits.  If we use the digits 1-9,
// then our board is nine units wide and nine units tall.  Furthermore,
// the number of digits should be a square number so that we can divide
// the board into boxes.  Nine digits means three boxes across and three
// boxes down.


public static class SudokuMatrix
{
    // row [1..dimension], column [1..dimension], digit [1..dimension]
    private static int GetRowIndex(int dimension, int row, int column, int digit)
    {
        return (row - 1) * dimension * dimension + (column - 1) * dimension + (digit - 1);
    }


    // row [1..dimension], column [1..dimension], digit [1..dimension]
    private static (int Row, int Column, int Digit) GetRowColumnDigitFromRowIndex(int dimension, int rowIndex)
    {
        int digit = (rowIndex % dimension) + 1;
        int column = (rowIndex / dimension) % dimension + 1;
        int row = (rowIndex / dimension) / dimension + 1;

        return (row, column, digit);
    }


    private static int GetNumberOfColumnsForDimension(int dimension)
    {
        // We need d*d columns for the row/column constraint.
        // We need d*d columns for the row/digit constraint.
        // We need d*d columns for the column/digit constraint.
        // We need d*d columns for the box/digit constraint.

        return dimension * dimension * 4;
    }


    private static int GetNumberOfRowsForDimension(int dimension)
    {
        // Each row and column can have any of the digits.
        // The number of rows is equal to the dimension.
        // So is the number of columns.
        // So is the number of digits.

        return dimension * dimension * dimension;
    }


    private static int GetDimensionFromNumberOfColumns(int numberOfColumns)
    {
        // We need d*d columns for the row/column constraint.
        // We need d*d columns for the row/digit constraint.
        // We need d*d columns for the column/digit constraint.
        // We need d*d columns for the box/digit constraint.

        return (int) Math.Sqrt(numberOfColumns / 4);
    }


    private static int GetDimensionFromNumberOfRows(int numberOfRows)
    {
        // Each row and column can have any of the digits.
        // The number of rows is equal to the dimension.
        // So is the number of columns.
        // So is the number of digits.

        // 1/3 can't be represented exactly in binary, so we won't get the exact answer we want.
        // We can fix it with a little rounding.

        return (int) Math.Round(Math.Pow(numberOfRows, 1.0 / 3.0));
    }


    public static bool[/* col */, /* row */] MakePlainMatrix(int dimension)
    {
        int numberOfColumns = GetNumberOfColumnsForDimension(dimension);
        int numberOfRows = GetNumberOfRowsForDimension(dimension);
        int boxDimension = (int) Math.Sqrt(dimension);

        bool[,] matrix = new bool[numberOfColumns, numberOfRows];

        int column = 0;

        // row-column constraints

        for (int y = 1; y <= dimension; ++y)
        {
            for (int x = 1; x <= dimension; ++x)
            {
                for (int d = 1; d <= dimension; ++d)
                {
                    matrix[column, GetRowIndex(dimension, y, x, d)] = true;
                }

                ++column;
            }
        }

        // row-digit constraints

        for (int y = 1; y <= dimension; ++y)
        {
            for (int d = 1; d <= dimension; ++d)
            {
                for (int x = 1; x <= dimension; ++x)
                {
                    matrix[column, GetRowIndex(dimension, y, x, d)] = true;
                }

                ++column;
            }
        }

        // column-digit constraints

        for (int x = 1; x <= dimension; ++x)
        {
            for (int d = 1; d <= dimension; ++d)
            {
                for (int y = 1; y <= dimension; ++y)
                {
                    matrix[column, GetRowIndex(dimension, y, x, d)] = true;
                }

                ++column;
            }
        }

        // box-digit constraints

        for (int boxY = 1; boxY <= dimension; boxY += boxDimension)
        {
            for (int boxX = 1; boxX <= dimension; boxX += boxDimension)
            {
                for (int d = 1; d <= dimension; ++d)
                {
                    for (int deltaY = 0; deltaY < boxDimension; ++deltaY)
                    {
                        for (int deltaX = 0; deltaX < boxDimension; ++deltaX)
                        {
                            matrix[column, GetRowIndex(dimension, boxY + deltaY, boxX + deltaX, d)] = true;
                        }
                    }

                    ++column;
                }
            }
        }

        return matrix;
    }


    public static bool[/* col */, /* row */] MakeMatrix(int[/* row */][/* col */] inputs)
    {
        int dimension = inputs.Length;

        int numberOfColumns = GetNumberOfColumnsForDimension(dimension);

        bool[,] matrix = MakePlainMatrix(dimension);

        for (int y = 1; y <= dimension; ++y)
        {
            Debug.Assert(inputs[y - 1].Length == dimension);

            for (int x = 1; x <= dimension; ++x)
            {
                int inputDigit = inputs[y - 1][x - 1];

                if (inputDigit != 0)
                {
                    // Since we know what digit goes here, let's zero out the constraints columns for all the
                    // *other* digits that could go here.  That way, they can never be selected as part of
                    // the solution.

                    for (int d = 1; d <= dimension; ++d)
                    {
                        if (d != inputDigit)
                        {
                            int matrixRowIndex = GetRowIndex(dimension, y, x, d);

                            for (int column = 0; column < numberOfColumns; ++column)
                            {
                                matrix[column, matrixRowIndex] = false;
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
        int dimension = GetDimensionFromNumberOfColumns(matrix.GetLength(0));
        Debug.Assert(dimension == GetDimensionFromNumberOfRows(matrix.GetLength(1)));

        int[/* row */][/* col */] output = new int[dimension][];

        for (int y = 0; y < dimension; ++y)
        {
            output[y] = new int[dimension];
        }

        foreach (int solutionRow in solution)
        {
            (int row, int column, int digit) = GetRowColumnDigitFromRowIndex(dimension, solutionRow);

            output[row - 1][column - 1] = digit;
        }

        return output;
    }


}

