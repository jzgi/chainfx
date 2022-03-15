using System;

namespace SkyChain.Chain
{
    /// <summary>
    /// Thrown to indicate a blockchain federal network related error.
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

        public FedException(string msg) : base(msg)
        {
        }

        public FedException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}