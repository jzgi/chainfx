using System;

namespace SkyChain.Db
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

        public ChainException(string msg) : base(msg)
        {
        }

        public ChainException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}