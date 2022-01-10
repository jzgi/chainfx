using System;

namespace SkyChain.Chain
{
    /// <summary>
    /// Thrown to indicate a blockchain related error.
    /// </summary>
    public class ChainException : Exception
    {
        /// <summary>
        /// The returned status code.
        /// </summary>
        public int Code { get; internal set; }

        public ChainException()
        {
        }

        public ChainException(string message) : base(message)
        {
        }

        public ChainException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}