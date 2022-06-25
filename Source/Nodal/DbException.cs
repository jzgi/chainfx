using System;

namespace DoChain.Nodal
{
    /// <summary>
    /// Thrown to indicate an error during database operation.
    /// </summary>
    public class DbException : Exception
    {
        /// <summary>
        /// The returned error code.
        /// </summary>
        public int Code { get; internal set; }

        public DbException()
        {
        }

        public DbException(string message) : base(message)
        {
        }

        public DbException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}