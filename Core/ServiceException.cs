using System;

namespace Greatbone.Core
{
    ///
    /// Thrown to indicate an illegal structuring in a service folder hierarchy.
    ///
    public class ServiceException : Exception
    {
        public ServiceException() { }

        public ServiceException(string message) : base(message) { }

        public ServiceException(string message, Exception inner) : base(message, inner) { }
    }
}