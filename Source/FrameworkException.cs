using System;

namespace SkyCloud
{
    ///
    /// Thrown to indicate an illegal structuring in a work hierarchy.
    ///
    public class FrameworkException : Exception
    {
        public FrameworkException()
        {
        }

        public FrameworkException(string msg) : base(msg)
        {
        }

        public FrameworkException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}