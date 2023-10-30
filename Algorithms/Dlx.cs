using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using static Pentomino.Algorithms.Dlx;

namespace Pentomino.Algorithms;


public class Dlx
{
    [System.Diagnostics.DebuggerDisplay("column = {m_column.m_name}, row = {m_row}")]
    private class Node
    {
        public Node m_left;
        public Node m_right;
        public Node m_above;
        public Node m_below;

        public ColumnNode m_column;

        public int m_row;

        public Node(ColumnNode column, int row)
        {
            m_row = row;
            m_column = column;

            // All connecting references point to ourself, initially.  That makes sense, since rows and columns wrap around.  
            // So a lone node wraps around to itself.

            m_left = m_right = m_above = m_below = this;
        }
    }


    [System.Diagnostics.DebuggerDisplay("column = {m_name}, size = {m_size}")]
    private class ColumnNode : Node
    {
        public string m_name;

        public int m_size;

        public ColumnNode(string name)
            : base(null!, -1)
        {
            m_name = name;
            m_size = 0;

            m_column = this;  // column header nodes point to themselves
        }
    }


    public class ProgressMetrics
    {
        // Data to be read by anybody, written only by Dlx solver.

        private long m_solutionCount = 0;
    
        public long SolutionCount { get => m_solutionCount; set => m_solutionCount = value; }


        private long m_rowsRemoved = 0;

        // Count to be read by anybody, written only by Dlx solver.
        public long RowsRemoved { get => m_rowsRemoved; set => m_rowsRemoved = value; }
    }


    private readonly ColumnNode m_root = new("root");

    private readonly HashSet<int> m_solution = new();  // current solution in progress

    private readonly List<HashSet<int>> m_solutions = new();  // All solutions found


    private Dlx(bool[/* col */, /* row */] matrix)
    {
        int numberOfColumns = matrix.GetLength(0);
        ColumnNode[] columns = new ColumnNode[numberOfColumns];

        // Create the column header nodes

        for (int jj = 0; jj < numberOfColumns; ++jj)
        {
            columns[jj] = new(jj.ToString());
            LinkNodeToRow(m_root.m_left, columns[jj]);
        }

        // Add all the matrix values as nodes

        int numberOfRows = matrix.GetLength(1);

        for (int jj = 0; jj < numberOfRows; ++jj)
        {
            Node? mostRecentNodeInCurrentRow = null;

            for (int kk = 0; kk < numberOfColumns; ++kk)
            {
                if (matrix[kk, jj] == true)  // We only create nodes for cells that contain a value
                {
                    ColumnNode columnHeader = columns[kk];

                    Node node = new(columnHeader, jj);

                    LinkNodeToColumn(columnHeader.m_above, node);

                    if (mostRecentNodeInCurrentRow is not null)
                        LinkNodeToRow(mostRecentNodeInCurrentRow, node);

                    mostRecentNodeInCurrentRow = node;
                }
            }
        }
    }


    private static void LinkNodeToRow(Node appendTo, Node nodeToAppend)
    {
        nodeToAppend.m_left = appendTo;
        nodeToAppend.m_right = appendTo.m_right;
        appendTo.m_right.m_left = nodeToAppend;
        appendTo.m_right = nodeToAppend;
    }


    private static void LinkNodeToColumn(Node appendTo, Node nodeToAppend)
    {
        nodeToAppend.m_above = appendTo;
        nodeToAppend.m_below = appendTo.m_below;
        appendTo.m_below.m_above = nodeToAppend;
        appendTo.m_below = nodeToAppend;

        ++appendTo.m_column.m_size;
    }


    private static void Cover(ColumnNode column, ProgressMetrics progressMetrics)
    {
        // unlink column
        column.m_right.m_left = column.m_left;
        column.m_left.m_right = column.m_right;

        // for each row in the column
        for (Node row = column.m_below; row != column; row = row.m_below)
        {
            ++progressMetrics.RowsRemoved;

            // for each node in the row
            for (Node right = row.m_right; right != row; right = right.m_right)
            {
                // unlink the node
                right.m_above.m_below = right.m_below;
                right.m_below.m_above = right.m_above;

                --right.m_column.m_size;
            }
        }
    }


    private static void Uncover(ColumnNode column)
    {
        // for each row in the column, in reverse order
        for (Node row = column.m_above; row != column; row = row.m_above)
        {
            // for each node in the row, in reverse order
            for (Node node = row.m_left; node != row; node = node.m_left)
            {
                ++node.m_column.m_size;

                // relink the node
                node.m_above.m_below = node;
                node.m_below.m_above = node;
            }
        }

        // relink column
        column.m_right.m_left = column;
        column.m_left.m_right = column;
    }


    private ColumnNode FindSmallestColumn()
    {
        int minSize = int.MaxValue;
        ColumnNode? minColumn = null;

        for (ColumnNode column = (ColumnNode) m_root.m_right; column != m_root; column = (ColumnNode) column.m_right)
        {
            if (column.m_size < minSize)
            {
                minSize = column.m_size;
                minColumn = column;
            }
        }

        return minColumn!;
    }


    private void Search(ProgressMetrics progressMetrics, CancellationToken cancelToken)
    {
        if (cancelToken.IsCancellationRequested)
            return;

        if (m_root.m_right == m_root)
        {
            // Since there are no more columns, we must have covered everything.

            m_solutions.Add(m_solution.ToHashSet());
            ++progressMetrics.SolutionCount;

            return;
        }

        // Otherwise, we need to keep looking.  We'll start by considering the column with the
        // fewest values in it.  This will allow us to rule out bad rows more quickly.

        ColumnNode column = this.FindSmallestColumn();
        if (column.m_size == 0)
            return;  // No more options in this column, so this search is a bust.

        Cover(column, progressMetrics);

        for (Node row = column.m_below; row != column; row = row.m_below)
        {
            if (cancelToken.IsCancellationRequested)
                break;

            m_solution.Add(row.m_row);  // propose a solution

            // for each node in the row
            for (Node right = row.m_right; right != row; right = right.m_right)
            {
                Cover(right.m_column, progressMetrics);  // Cover the column
            }

            this.Search(progressMetrics, cancelToken);  // recurse

            // backtrack (reject the proposed solution)
            m_solution.Remove(row.m_row);
            column = row.m_column;

            // for each node in reverse order
            for (Node left = row.m_left; left != row; left = left.m_left)
            {
                Uncover(left.m_column);  // Uncover the column
            }
        }

        Uncover(column);
    }


    public static List<HashSet<int>> Solve(bool[/* col */, /* row */] matrix, CancellationToken cancelToken, ProgressMetrics? progressMetrics = null)
    {
        progressMetrics ??= new();
        progressMetrics.SolutionCount = 0;

        Dlx dlx = new(matrix);
        dlx.Search(progressMetrics, cancelToken);

        return dlx.m_solutions;
    }


}
