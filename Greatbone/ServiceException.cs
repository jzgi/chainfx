using System;

namespace Greatbone
{
    ///
    /// Thrown to indicate an illegal structuring in a work hierarchy.
    ///
    public class ServiceException : Exception
    {
        public ServiceException()
        {
        }

        public ServiceException(string message) : base(message)
        {
        }

        public ServiceException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}