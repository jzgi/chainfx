using System;

namespace SkyChain.Nodal
{
    /// <summary>
    /// Thrown to indicate a blockchain federal network related error.
    /// </summary>
    public class NodeException : Exception
    {
        /// <summary>
        /// The returned status code.
        /// </summary>
        public int Code { get; internal set; }

        public NodeException()
        {
        }

        public NodeException(string msg) : base(msg)
        {
        }

        public NodeException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}