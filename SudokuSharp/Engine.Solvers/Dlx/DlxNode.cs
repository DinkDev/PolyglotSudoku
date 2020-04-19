namespace SudokuSharp.Engine.Solvers.Dlx
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Dlx support - inspired by Miran Uhan's Dancing Links Library in Codeproject/
    /// http://www.codeproject.com/Articles/19630/Dancing-Links-Library
    /// </summary>
    internal class DlxNode
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="coord">The coordinate for this node</param>
        public DlxNode(PuzzleCoordinate coord)
        {
            Coordinate = coord;

            Header = null;
            Left = this;
            Right = this;
            Up = this;
            Down = this;
        }

        public PuzzleCoordinate Coordinate { get; }

        public DlxHeader Header { get; set; }
        public DlxNode Left { get; set; }
        public DlxNode Right { get; set; }
        public DlxNode Up { get; set; }
        public DlxNode Down { get; set; }

        /// <summary>
        /// Method to iterate DlxNodes. This allows foreach/IEnumerable usage when walking DlxNode references Left, Right, Up or Down!
        /// </summary>
        /// <param name="next">Func to move to next node, like: r =&gt; r.Left</param>
        /// <param name="moreToFind">Func for additional option to stop iteration.</param>
        /// <returns>All the DlxNodes in the traversal (up until moreToFind returns false).</returns>
        /// <remarks>
        /// Adapted from a brilliant idea posted at: https://gist.github.com/Eibwen/e8fec52e0f5927108d86
        /// By Eibwen/Greg Walker
        /// </remarks>
        public IEnumerable<DlxNode> EnumerateList(Func<DlxNode, DlxNode> next, Func<bool> moreToFind)
        {
            var node = this;

            while ((node = next(node)) != this && moreToFind())
            {
                yield return node;
            }
        }

        /// <summary>
        /// Method to iterate DlxNodes. This allows foreach/IEnumerable usage when walking DlxNode references Left, Right, Up or Down!
        /// </summary>
        /// <param name="next">Func to move to next node, like: r =&gt; r.Left</param>
        /// <returns>All the DlxNodes in the traversal.</returns>
        /// <remarks>
        /// Adapted from a brilliant idea by Eibwen/Greg Walker.
        /// <see cref="http://gist.github.com/Eibwen/e8fec52e0f5927108d86"/>
        /// </remarks>
        public IEnumerable<DlxNode> EnumerateList(Func<DlxNode, DlxNode> next)
        {
            return EnumerateList(next, () => true);
        }
    }
}
