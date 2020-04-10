using System;

namespace SkyCloud.Chain
{
    ///
    /// Thrown to indicate a chain-related error.
    ///
    public class ChainException : Exception
    {
        /// <summary>
        /// The status code.
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