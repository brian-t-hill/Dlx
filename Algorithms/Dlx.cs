using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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

            // All connecting references point to ourself, initially.
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


    private readonly ColumnNode m_root;

    private readonly HashSet<int> m_solution;  // current solution in progress

    private readonly List<HashSet<int>> m_solutions;  // All solutions found

    private Dlx(bool[/* col */, /* row */] matrix)
    {
        m_root = new("root");
        m_solution = new();
        m_solutions = new();

        int numberOfColumns = matrix.GetLength(0);

        ColumnNode[] columns = new ColumnNode[numberOfColumns];

        for (int jj = 0; jj < numberOfColumns; ++jj)
        {
            columns[jj] = new(jj.ToString());
            AppendNodeToRow(m_root.m_left, columns[jj]);
        }

        int numberOfRows = matrix.GetLength(1);

        for (int jj = 0; jj < numberOfRows; ++jj)
        {
            Node? mostRecentInRow = null;

            for (int kk = 0; kk < numberOfColumns; ++kk)
            {
                if (matrix[kk, jj] == true)  // We only create nodes for cells that contain a value
                {
                    Node node = new(columns[kk], jj);

                    AppendNodeToColumn(columns[kk].m_above, node);

                    ++columns[kk].m_size;

                    if (mostRecentInRow is not null)
                        AppendNodeToRow(mostRecentInRow, node);

                    mostRecentInRow = node;
                }
            }
        }
    }


    private static void AppendNodeToRow(Node appendTo, Node nodeToAppend)
    {
        nodeToAppend.m_left = appendTo;
        nodeToAppend.m_right = appendTo.m_right;
        appendTo.m_right.m_left = nodeToAppend;
        appendTo.m_right = nodeToAppend;
    }


    private static void AppendNodeToColumn(Node appendTo, Node nodeToAppend)
    {
        nodeToAppend.m_above = appendTo;
        nodeToAppend.m_below = appendTo.m_below;
        appendTo.m_below.m_above = nodeToAppend;
        appendTo.m_below = nodeToAppend;
    }


    private static void Cover(ColumnNode column)
    {
        // unlink column
        column.m_right.m_left = column.m_left;
        column.m_left.m_right = column.m_right;

        // for each row in the column
        for (Node row = column.m_below; row != column; row = row.m_below)
        {
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
            for (Node left = row.m_left; left != row; left = left.m_left)
            {
                ++left.m_column.m_size;

                // relink the node
                left.m_above.m_below = left;
                left.m_below.m_above = left;
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


    private void Search(CancellationToken cancelToken)
    {
        if (cancelToken.IsCancellationRequested)
            return;

        if (m_root.m_right == m_root)
        {
            // Since there are no more columns, we must have covered everything.

            m_solutions.Add(m_solution.ToHashSet());

            return;
        }

        // Otherwise, we need to keep looking.

        ColumnNode column = this.FindSmallestColumn();
        if (column.m_size == 0)
            return;  // No more options in this column, so this search is a bust.

        Cover(column);

        for (Node row = column.m_below; row != column; row = row.m_below)
        {
            if (cancelToken.IsCancellationRequested)
                break;

            m_solution.Add(row.m_row);  // propose a solution

            // for each node in the row
            for (Node right = row.m_right; right != row; right = right.m_right)
            {
                Cover(right.m_column);  // Cover the column
            }

            this.Search(cancelToken);  // recurse

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


    private List<HashSet<int>> Solve(CancellationToken cancelToken)
    {
        this.Search(cancelToken);

        return m_solutions;
    }


    public static List<HashSet<int>> Solve(bool[/* col */, /* row */] matrix, CancellationToken cancelToken)
    {
        Dlx dlx = new(matrix);

        return dlx.Solve(cancelToken);
    }


}
