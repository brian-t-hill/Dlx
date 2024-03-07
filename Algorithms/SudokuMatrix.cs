using Pentomino.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Pentomino.Algorithms;

// Sudoku is defined by the number of digits.  If we use the digits 1-9,
// then our board is nine units wide and nine units tall.  Furthermore,
// the number of digits should be a square number so that we can divide
// the board into boxes.  Nine digits means three boxes across and three
// boxes down.


public static class SudokuMatrix
{

    // row [1..dimension], column [1..dimension], digit [1..dimension]
    private static int GetRowIndex(int dimension, int row, int column, int digit, int[]? shuffledRowIndexes)
    {
        int rowIndex = (row - 1) * dimension * dimension + (column - 1) * dimension + (digit - 1);

        if (shuffledRowIndexes is not null)
            return shuffledRowIndexes[rowIndex];

        return rowIndex;
    }


    // row [1..dimension], column [1..dimension], digit [1..dimension]
    private static (int Row, int Column, int Digit) GetRowColumnDigitFromRowIndex(int dimension, int rowIndex, int[]? deshuffledRowIndexes)
    {
        if (deshuffledRowIndexes is not null)
            rowIndex = deshuffledRowIndexes[rowIndex];

        int digit = (rowIndex % dimension) + 1;
        int column = (rowIndex / dimension) % dimension + 1;
        int row = (rowIndex / dimension) / dimension + 1;

        return (row, column, digit);
    }


    private static int GetNumberOfMatrixColumnsForDimension(int dimension)
    {
        // We need d*d columns for the row/column constraint.
        // We need d*d columns for the row/digit constraint.
        // We need d*d columns for the column/digit constraint.
        // We need d*d columns for the box/digit constraint.

        return dimension * dimension * 4;
    }


    private static int GetNumberOfMatrixRowsForDimension(int dimension)
    {
        // Each Sudoku row and column can have any of the digits.
        // The number of Sudoku rows is equal to the dimension.
        // So is the number of Sudoku columns.
        // So is the number of digits.

        return dimension * dimension * dimension;
    }


    private static int GetDimensionFromNumberOfMatrixColumns(int numberOfColumns)
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


    public static List<bool[]> MakePlainMatrix(int dimension, int[]? shuffledRowIndexes)
    {
        int numberOfMatrixColumns = GetNumberOfMatrixColumnsForDimension(dimension);
        int numberOfMatrixRows = GetNumberOfMatrixRowsForDimension(dimension);
        int boxDimension = (int) Math.Sqrt(dimension);

        List<bool[]> matrixRows = Enumerable.Repeat(0, numberOfMatrixRows).Select(n => new bool[numberOfMatrixColumns]).ToList();

        int matrixColumnIndex = 0;

        // row-column constraints

        for (int y = 1; y <= dimension; ++y)
        {
            for (int x = 1; x <= dimension; ++x)
            {
                for (int d = 1; d <= dimension; ++d)
                {
                    matrixRows[GetRowIndex(dimension, y, x, d, shuffledRowIndexes)][matrixColumnIndex] = true;
                }

                ++matrixColumnIndex;
            }
        }

        // row-digit constraints

        for (int y = 1; y <= dimension; ++y)
        {
            for (int d = 1; d <= dimension; ++d)
            {
                for (int x = 1; x <= dimension; ++x)
                {
                    matrixRows[GetRowIndex(dimension, y, x, d, shuffledRowIndexes)][matrixColumnIndex] = true;
                }

                ++matrixColumnIndex;
            }
        }

        // column-digit constraints

        for (int x = 1; x <= dimension; ++x)
        {
            for (int d = 1; d <= dimension; ++d)
            {
                for (int y = 1; y <= dimension; ++y)
                {
                    matrixRows[GetRowIndex(dimension, y, x, d, shuffledRowIndexes)][matrixColumnIndex] = true;
                }

                ++matrixColumnIndex;
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
                            matrixRows[GetRowIndex(dimension, boxY + deltaY, boxX + deltaX, d, shuffledRowIndexes)][matrixColumnIndex] = true;
                        }
                    }

                    ++matrixColumnIndex;
                }
            }
        }

        return matrixRows;
    }


    public static List<bool[]> MakeMatrix(int[/* row */][/* col */] inputs, int[]? shuffledRowIndexes)
    {
        int dimension = inputs.Length;

        int numberOfMatrixColumns = GetNumberOfMatrixColumnsForDimension(dimension);

        List<bool[]> matrixRows = MakePlainMatrix(dimension, shuffledRowIndexes);

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
                            int matrixRowIndex = GetRowIndex(dimension, y, x, d, shuffledRowIndexes);

                            for (int matrixColumnIndex = 0; matrixColumnIndex < numberOfMatrixColumns; ++matrixColumnIndex)
                            {
                                matrixRows[matrixRowIndex][matrixColumnIndex] = false;
                            }
                        }
                    }
                }
            }
        }

        return matrixRows;
    }


    // CreateValidCompletedSudokuInputs
    //
    // This creates a valid Sudoku board, with all digits filled in.  The way it works is that we
    // run the Dlx solver on an empty input to find solutions.  We limit the number of solutions
    // to 1, so it will find a solution quickly.  In order to generate a different solution every
    // time, we randomize the matrix rows.  The problem with that is that the row indexes are
    // actually important, since they encode the row, column, and digit placement.  So we leave
    // the matrix intact, but we randomize access to the rows by using a shuffled array of
    // indexes, and then we have a complementary array to map back.
    //
    public static int[/* row */][/* col */] CreateValidCompletedSudokuInputs(int dimension)
    {
        int[/* row */][/* col */] emptyInputs = new int[dimension][];

        for (int y = 0; y < dimension; ++y)
        {
            emptyInputs[y] = new int[dimension];
        }

        int[] shuffledRowIndexes = Enumerable.Range(0, GetNumberOfMatrixRowsForDimension(dimension)).ToArray().Shuffle();
        int[] deshuffledRowIndexes = new int[shuffledRowIndexes.Length];

        for (int jj = 0; jj < deshuffledRowIndexes.Length; ++jj)
        {
            deshuffledRowIndexes[shuffledRowIndexes[jj]] = jj;
        }

        List<bool[]> matrixRows = MakeMatrix(emptyInputs, shuffledRowIndexes);

        Dlx.ProgressMetrics progressMetrics = new() { SolutionLimit = 1 }; 
        Dlx.Solve(matrixRows, parallelSolver: false, CancellationToken.None, progressMetrics);

        List<HashSet<int>> solutions = progressMetrics.GetConfirmedSolutions();
        Debug.Assert(solutions.Count == 1);

        return MakeOutputsFromSolution(solutions.First(), dimension, deshuffledRowIndexes);
    }


    // ReduceSolutionToOpeningClues
    //
    // At the beginning, inputs must contain an entire valid solution.  This function will remove
    // values until it can guarantee that only one solution can be derived from the remaining inputs.
    //
    public static void ReduceSolutionToOpeningClues(int[/* row */][/* col */] inputs)
    {
        int numberOfRows = inputs.Length;
        int numberOfColumns = inputs[0].Length;

        List<int> randomizedSpaces = Enumerable.Range(0, numberOfRows * numberOfColumns).ToList().Shuffle();

        while (randomizedSpaces.Count > 0)
        {
            // A full, completed grid will have only one solution, of course.  Starting there, we'll pick a
            // random digit and remove it.  Then we'll test to see if there is still just a single solution.
            // If there is, we may not have removed enough, so we'll loop back and remove another digit.
            // But if there are multiple solutions, then we'll put the digit back and loop back to try
            // removing another digit.  We'll continue until we've tried removing all the digits.

            int space = randomizedSpaces[^1];
            randomizedSpaces.RemoveAt(randomizedSpaces.Count - 1);

            int row = space / numberOfColumns;
            int column = space % numberOfColumns;

            int removedDigit = inputs[row][column];
            inputs[row][column] = 0;

            List<bool[]> matrixRows = Algorithms.SudokuMatrix.MakeMatrix(inputs, shuffledRowIndexes: null);
            Dlx.ProgressMetrics progressMetrics = new() { SolutionLimit = 2, CountSolutionsWithoutCollecting = true };
            Algorithms.Dlx.Solve(matrixRows, parallelSolver: false, CancellationToken.None, progressMetrics);

            if (progressMetrics.SolutionCount > 1)
                inputs[row][column] = removedDigit;
        }
    }


    public static int[/* row */][/* col */] GenerateRandomOpeningInputs(int dimension)
    {
        int[/* row */][/* col */] inputs = CreateValidCompletedSudokuInputs(dimension);
        ReduceSolutionToOpeningClues(inputs);

        return inputs;
    }


    public static int[/* row */][/* col */] MakeOutputsFromSolution(HashSet<int> solution, int dimension, int[]? deshuffledRowIndexes)
    {
        int[/* row */][/* col */] output = new int[dimension][];

        for (int y = 0; y < dimension; ++y)
        {
            output[y] = new int[dimension];
        }

        foreach (int solutionRow in solution)
        {
            (int row, int column, int digit) = GetRowColumnDigitFromRowIndex(dimension, solutionRow, deshuffledRowIndexes);

            output[row - 1][column - 1] = digit;
        }

        return output;
    }


}

