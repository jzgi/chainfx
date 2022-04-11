using System;

namespace Chainly.Nodal
{
    /// <summary>
    /// Thrown to indicate a blockchain federal network related error.
    /// </summary>
    public class NodalException : Exception
    {
        /// <summary>
        /// The returned status code.
        /// </summary>
        public int Code { get; internal set; }

        public NodalException()
        {
        }

        public NodalException(string msg) : base(msg)
        {
        }

        public NodalException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}