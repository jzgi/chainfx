using System;

namespace CoChain.Web
{
    ///
    /// Thrown to indicate an illegal structuring in a work hierarchy.
    ///
    public class ApplicationException : Exception
    {
        public ApplicationException()
        {
        }

        public ApplicationException(string msg) : base(msg)
        {
        }

        public ApplicationException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}