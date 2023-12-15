using System;

namespace ChainFX.Web
{
    ///
    /// Thrown to indicate an illegal structuring in a work hierarchy.
    ///
    public class ForbiddenException : Exception
    {
        public static readonly ForbiddenException NoPrincipalError = new ForbiddenException("No principal");

        public static readonly ForbiddenException AccessorReq = new ForbiddenException("Accessor required");


        public ForbiddenException()
        {
        }

        public ForbiddenException(string msg) : base(msg)
        {
        }

        public ForbiddenException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}