using System;

namespace SkyChain.Store
{
    /// <summary>
    /// Thrown to indicate a blockchain related error.
    /// </summary>
    public class FedException : Exception
    {
        /// <summary>
        /// The returned status code.
        /// </summary>
        public int Code { get; internal set; }

        public FedException()
        {
        }

        public FedException(string message) : base(message)
        {
        }

        public FedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}