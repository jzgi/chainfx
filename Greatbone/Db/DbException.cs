using System;

namespace WebReady.Db
{
    ///
    /// Thrown to indicate an error during a database operation.
    ///
    public class DbException : Exception
    {
        /// <summary>
        /// The error code.
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