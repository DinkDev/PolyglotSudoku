namespace SudokuSharp.Engine.Solvers.Dlx
{
    /// <summary>
    /// Dlx support - inspired by Miran Uhan's Dancing Links Library in CodeProject.
    /// http://www.codeproject.com/Articles/19630/Dancing-Links-Library
    /// </summary>
    internal class DlxHeader : DlxNode
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="column">The column number for this header</param>
        public DlxHeader(int column) : base(new PuzzleCoordinate(0, column))
        {
        }

        /// <summary>
        /// Simplify access.
        /// </summary>
        public int Column => Coordinate.Col;

        public int NodeCount { get; set; }

        /// <summary>
        /// Add a new row Dlx node to the end of a column.
        /// </summary>
        /// <param name="newNode"></param>
        public void AddNode(DlxNode newNode)
        {
            var upNode = Up;
            newNode.Up = upNode;
            newNode.Down = this;
            upNode.Down = newNode;
            Up = newNode;

            NodeCount++;
        }

        /// <summary>
        /// Part of the Dlx algorithm that covers the column header
        /// </summary>
        public void Cover()
        {
            // remove header
            Left.Right = Right;
            Right.Left = Left;

            foreach (var columnNode in EnumerateList(c => c.Down))
            {
                // remove nodes
                foreach (var currentNode in columnNode.EnumerateList(r => r.Right))
                {
                    currentNode.Up.Down = currentNode.Down;
                    currentNode.Down.Up = currentNode.Up;
                    currentNode.Header.NodeCount--;
                }
            }
        }

        /// <summary>
        /// Part of the Dlx algorithm that uncovers the column header
        /// </summary>
        public void Uncover()
        {
            // replace header
            Left.Right = this;
            Right.Left = this;

            foreach (var columnNode in EnumerateList(c => c.Up))
            {
                // replace nodes
                foreach (var currentNode in columnNode.EnumerateList(r => r.Right))
                {
                    currentNode.Up.Down = currentNode;
                    currentNode.Down.Up = currentNode;
                    currentNode.Header.NodeCount++;
                }
            }
        }

        /// <summary>
        /// Searches DlxHeaders for the one with the fewest nodes.
        /// </summary>
        /// <returns></returns>
        public DlxHeader FindFewestNodes(bool skipCurrent = true)
        {
            var rv = this;
            if (skipCurrent)
            {
                rv = (DlxHeader)Left;
            }
            var minNodeCount = rv.NodeCount;

            for (var currentHdr = (DlxHeader)Right;
                !Equals(currentHdr, Left);
                currentHdr = (DlxHeader)currentHdr.Right)
            {
                if (currentHdr.NodeCount < minNodeCount)
                {
                    rv = currentHdr;
                    minNodeCount = currentHdr.NodeCount;
                }
            }

            return rv;
        }

        /// <summary>
        /// Finds if this is the only column header.
        /// </summary>
        /// <returns></returns>
        public bool IsSingle()
        {
            return Equals(this, Right);
        }
    }
}