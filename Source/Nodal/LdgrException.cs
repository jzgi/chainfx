using System;

namespace Chainly.Nodal
{
    /// <summary>
    /// Thrown to indicate a blockchain federal network related error.
    /// </summary>
    public class LdgrException : Exception
    {
        /// <summary>
        /// The returned status code.
        /// </summary>
        public int Code { get; internal set; }

        public LdgrException()
        {
        }

        public LdgrException(string msg) : base(msg)
        {
        }

        public LdgrException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}