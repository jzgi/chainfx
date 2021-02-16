using System;

namespace SkyChain
{
    ///
    /// Thrown to indicate an illegal structuring in a work hierarchy.
    ///
    public class ServerException : Exception
    {
        public ServerException()
        {
        }

        public ServerException(string msg) : base(msg)
        {
        }

        public ServerException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}