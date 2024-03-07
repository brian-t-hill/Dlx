using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pentomino.Algorithms;


public class Dlx
{
    [System.Diagnostics.DebuggerDisplay("column = {m_column.m_name}, row = {m_row}")]
    private class Node
    {
        public int m_row;

        public ColumnNode m_column;

        public Node m_left;
        public Node m_right;
        public Node m_above;
        public Node m_below;

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
        public string m_name;  // Just for convenience

        public int m_size;  // How many regular nodes are in this column

        public ColumnNode(string name)
            : base(null!, -1)
        {
            m_name = name;
            m_size = 0;

            m_column = this;
        }
    }


    public class ProgressMetrics
    {
        private long m_solutionLimit = long.MaxValue;

        public long SolutionLimit
        {
            get => m_solutionLimit;
            init => m_solutionLimit = value;
        }

        public bool SolutionLimitReached() => this.SolutionLimit <= this.SolutionCount;


        private bool m_countSolutionsWithoutCollecting = false;

        public bool CountSolutionsWithoutCollecting
        {
            get => m_countSolutionsWithoutCollecting;
            init => m_countSolutionsWithoutCollecting = value;
        }


        public void ResetAll()
        {
            // Not thread-safe.

            this.ResetSolutionCount();
            this.ResetRowsRemoved();
            this.ResetRecursivePasses();
            this.m_confirmedSolutions.Clear();
        }


        private long m_solutionCount = 0;
    
        public long SolutionCount => Interlocked.Read(ref m_solutionCount);

        private long IncrementSolutionCount() => Interlocked.Increment(ref m_solutionCount);

        public void ResetSolutionCount() => Interlocked.Exchange(ref m_solutionCount, 0);


        private long m_rowsRemoved = 0;

        public long RowsRemoved => Interlocked.Read(ref m_rowsRemoved);

        public long IncrementRowsRemoved() => Interlocked.Increment(ref m_rowsRemoved);

        public void ResetRowsRemoved() => Interlocked.Exchange(ref m_rowsRemoved, 0);


        private long m_recursivePasses = 0;

        public long RecursivePasses => Interlocked.Read(ref m_recursivePasses);

        public long IncrementRecursivePasses() => Interlocked.Increment(ref m_recursivePasses);

        public void ResetRecursivePasses() => Interlocked.Exchange(ref m_recursivePasses, 0);


        private readonly object m_solutionLock = new();

        private readonly List<HashSet<int>> m_confirmedSolutions = new();  // All solutions found so far


        public void CollectSolution(HashSet<int> solution)
        {
            lock (m_solutionLock)
            {
                if (this.SolutionCount >= this.SolutionLimit)
                    return;

                this.IncrementSolutionCount();

                if (!this.CountSolutionsWithoutCollecting)
                {
                    // List<>.Count is an int, but m_solutionCount is a long.  That's okay.  That just means that
                    // we can *count* a lot more solutions than we can *collect*.

                    // REVIEW$:  When the number of solutions is large, the performance gets **REALLY**
                    // bad.  Undoubtedly from having to resize the large array.

                    m_confirmedSolutions.Add(solution);
                }
            }
        }

        public void CountSolutionWithoutCollecting() => this.IncrementSolutionCount();


        public List<HashSet<int>> GetConfirmedSolutions()
        {
            // Not thread-safe.

            return new(m_confirmedSolutions);
        }

    }


    private readonly ColumnNode m_root = new("root");

    private readonly HashSet<int> m_workingSolution = new();  // current solution in progress


    private Dlx(ColumnNode root, HashSet<int> workingSolution)
    {
        m_root = root;
        m_workingSolution = workingSolution;
    }


    private Dlx(List<bool[]> matrixRows)
    {
        if (matrixRows.Count == 0)
            return;

        int numberOfColumns = matrixRows[0].Length;
        ColumnNode[] columns = new ColumnNode[numberOfColumns];

        // Create the column header nodes

        for (int jj = 0; jj < numberOfColumns; ++jj)
        {
            columns[jj] = new(jj.ToString());
            LinkNodeToRow(m_root.m_left, columns[jj]);
        }

        // Add all the matrix values as nodes

        int numberOfRows = matrixRows.Count;

        for (int jj = 0; jj < numberOfRows; ++jj)
        {
            bool[] matrixRow = matrixRows[jj];
            Debug.Assert(matrixRow.Length == numberOfColumns);

            Node? mostRecentNodeInCurrentRow = null;

            for (int kk = 0; kk < numberOfColumns; ++kk)
            {
                if (matrixRow[kk] == true)  // We only create nodes for cells that contain a value
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


    private static (ColumnNode ClonedRoot, Node? ClonedNodeOfAttention) CloneMatrix(ColumnNode root, Node? nodeOfAttention)
    {
        ColumnNode clonedRoot = new(root.m_name);
        Dictionary<Node, Node> oldNodeToNewNodeMapping = new() { [root] = clonedRoot };

        // First, create a copy of each node without preserving any of the links.  Map each copy to its original.

        for (ColumnNode columnHeader = (ColumnNode) root.m_right; columnHeader != root; columnHeader = (ColumnNode) columnHeader.m_right)
        {
            ColumnNode clonedColumnHeader = new(columnHeader.m_name);
            oldNodeToNewNodeMapping.Add(columnHeader, clonedColumnHeader);

            for (Node node = columnHeader.m_below; node != columnHeader; node = node.m_below)
            {
                oldNodeToNewNodeMapping.Add(node, new Node(clonedColumnHeader, node.m_row));
            }
        }

        // Now, go through all the copied nodes and recreate their links.

        foreach (var kv in oldNodeToNewNodeMapping)
        {
            Node originalNode = kv.Key;
            Node clonedNode = kv.Value;

            clonedNode.m_left = oldNodeToNewNodeMapping[originalNode.m_left];
            clonedNode.m_right = oldNodeToNewNodeMapping[originalNode.m_right];
            clonedNode.m_above = oldNodeToNewNodeMapping[originalNode.m_above];
            clonedNode.m_below = oldNodeToNewNodeMapping[originalNode.m_below];

            ++clonedNode.m_column.m_size;
        }

        return (clonedRoot, nodeOfAttention is null ? null : oldNodeToNewNodeMapping[nodeOfAttention]);
    }


    private static (Dlx ClonedDlx, Node? ClonedNodeOfAttention) Clone(Dlx cloneFrom, Node? nodeOfAttention)
    {
        var (clonedRoot, clonedNodeOfAttention) = CloneMatrix(cloneFrom.m_root, nodeOfAttention);

        return (new Dlx(clonedRoot, new HashSet<int>(cloneFrom.m_workingSolution)), clonedNodeOfAttention);
    }


    private static void LinkNodeToRow(Node neighborToLeft, Node nodeToAdd)
    {
        nodeToAdd.m_left = neighborToLeft;
        nodeToAdd.m_right = neighborToLeft.m_right;
        neighborToLeft.m_right.m_left = nodeToAdd;
        neighborToLeft.m_right = nodeToAdd;
    }


    private static void LinkNodeToColumn(Node neighborAbove, Node nodeToAdd)
    {
        nodeToAdd.m_above = neighborAbove;
        nodeToAdd.m_below = neighborAbove.m_below;
        neighborAbove.m_below.m_above = nodeToAdd;
        neighborAbove.m_below = nodeToAdd;

        ++neighborAbove.m_column.m_size;
    }


    private static void CoverColumn(ColumnNode column, ProgressMetrics progressMetrics)
    {
        // I'm not thrilled with the name of this function, CoverColumn, but I guess it's as good as any
        // I've come up with.
        //
        // This function will remove the indicated column from the matrix, but that's not all.  Because
        // only one row is allowed to cover the column, we remove all other rows that also cover it.  We
        // don't actually know which row is supposed to be the one covering it, so we remove all of them.
        // That's okay because the algorithm will still read the column (even after it's removed) in
        // order to consider each of the covering rows, one at a time.

        // unlink column
        column.m_right.m_left = column.m_left;
        column.m_left.m_right = column.m_right;

        // for each row in the column
        for (Node row = column.m_below; row != column; row = row.m_below)
        {
            progressMetrics.IncrementRowsRemoved();

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


    private static void UncoverColumn(ColumnNode column)
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


    private static IEnumerable<Node> GetRowsAsEnumerable(ColumnNode column)
    {
        for (Node row = column.m_below; row != column; row = row.m_below)
        {
            yield return row;
        }
    }


    private void ConsiderRow(Node row, ProgressMetrics progressMetrics, CancellationToken cancelToken)
    {
        // This function was broken out of the Solve function.  It used to be the main body of the loop.
        // I've separated it to make it easier to call from both the regular solver and the parallel solver.

        m_workingSolution.Add(row.m_row);  // propose a solution

        // for each node in the row
        for (Node nodeInRow = row.m_right; nodeInRow != row; nodeInRow = nodeInRow.m_right)
        {
            CoverColumn(nodeInRow.m_column, progressMetrics);  // Cover the column
        }

        this.Solve(progressMetrics, cancelToken);  // recurse

        // backtrack (reject the proposed solution)
        m_workingSolution.Remove(row.m_row);

        // for each node in reverse order
        for (Node nodeInRow = row.m_left; nodeInRow != row; nodeInRow = nodeInRow.m_left)
        {
            UncoverColumn(nodeInRow.m_column);  // Uncover the column
        }
    }


    // Here's an attempt to parallize the solver.  I didn't spend a lot of time on it, though I did verify
    // that it works.  There are opportunities to improve it.  The strategy of the parallel solver is to
    // parallelize the loop of our top-level pass only.  There will be no further parallelization during
    // the recursive stages.  The recursive solver is destructive to the matrix, although it does repair
    // its damage before continuing.  Unfortunately, that means we can't use the same matrix when we
    // work on multiple rows at the same time.  Consequently, I clone the entire matrix for each parallel
    // iteration.  That negates at least some portion of the parallel benefits, but not all.  At least,
    // not with the matrixes in use here.
    //
    // One possible avenue for improvement might be to clone the matrix only once per thread and then
    // reuse that clone for all the work done on that thread.  That would require a queue of work for each
    // thread, ensuring that the passes queued for each thread never overlap.
    //
    private void ParallelSolve(ProgressMetrics progressMetrics, CancellationToken cancelToken)
    {
        progressMetrics.IncrementRecursivePasses();

        if (cancelToken.IsCancellationRequested)
            return;

        if (m_root.m_right == m_root)
        {
            // Since there are no more columns, we must have covered everything successfully. 

            progressMetrics.CollectSolution(m_workingSolution.ToHashSet());
            return;
        }

        // Otherwise, we need to keep looking.  We'll start by considering the column with the
        // fewest values in it.  This will allow us to rule out bad rows more quickly.

        ColumnNode column = this.FindSmallestColumn();
        if (column.m_size == 0)
            return;  // No more options in this column, so this solution is a bust.

        Parallel.ForEach(GetRowsAsEnumerable(column), (loopRowStart, loopState) =>
        {
            if (cancelToken.IsCancellationRequested)
            {
                loopState.Stop();
                return;
            }

            (Dlx clonedDlx, Node? clonedloopRowStart) = Dlx.Clone(this, loopRowStart);

            // In the non-parallel algorithm, we cover the column before we begin the loop.  That's because
            // it only needs to be done once, and the column is still available (though disconnected) for
            // the loop to use.
            // 
            // Here, however, we don't want the loop to work on the disconnected column.  We need the loop
            // to work on a clone of the column.  That means we can't disconnect it until after we clone
            // the matrix, which we do inside the loop.
            //
            // On the other hand, we don't have to worry about reconnecting the column when we're done,
            // since the cloned matrix goes out of scope as soon as we leave the loop.

            CoverColumn(clonedloopRowStart!.m_column, progressMetrics);

            clonedDlx.ConsiderRow(clonedloopRowStart, progressMetrics, cancelToken);

            if (progressMetrics.SolutionLimitReached())
            {
                loopState.Stop();
                return;
            }

            // As indicated above, there's no need to restore the covered column
            // UncoverColumn(clonedloopRowStart!.m_column);
        });

        // We've exhausted all the rows and still haven't covered all the columns, so this
        // combination's a bust.

        UncoverColumn(column);
    }


    // Solve, the recursive function
    //
    private void Solve(ProgressMetrics progressMetrics, CancellationToken cancelToken)
    {
        progressMetrics.IncrementRecursivePasses();

        if (cancelToken.IsCancellationRequested)
            return;

        if (m_root.m_right == m_root)
        {
            // Since there are no more columns, we must have covered everything successfully. 

            progressMetrics.CollectSolution(m_workingSolution.ToHashSet());
            return;
        }

        // Otherwise, we need to keep looking.  We'll start by considering the column with the
        // fewest values in it.  This will allow us to rule out bad rows more quickly.

        ColumnNode column = this.FindSmallestColumn();
        if (column.m_size == 0)
            return;  // No more options in this column, so this solution is a bust.

        CoverColumn(column, progressMetrics);

        for (Node row = column.m_below; row != column; row = row.m_below)
        {
            if (cancelToken.IsCancellationRequested)
                break;

            this.ConsiderRow(row, progressMetrics, cancelToken);

            if (progressMetrics.SolutionLimitReached())
                break;
        }

        // We've exhausted all the rows and still haven't covered all the columns, so this
        // combination's a bust.

        UncoverColumn(column);
    }


    public static void Solve(List<bool[]> matrixRows, bool parallelSolver, CancellationToken cancelToken, ProgressMetrics? progressMetrics = null)
    {
        progressMetrics ??= new();
        progressMetrics.ResetSolutionCount();

        if (progressMetrics.SolutionLimit <= 0)
            return;

        Dlx dlx = new(matrixRows);

        if (parallelSolver)
            dlx.ParallelSolve(progressMetrics, cancelToken);
        else
            dlx.Solve(progressMetrics, cancelToken);
    }


}
