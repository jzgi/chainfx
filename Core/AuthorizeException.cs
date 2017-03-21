using System;

namespace Greatbone.Core
{
    ///
    /// Thrown to indicate an failure of access check to a certain nodule. A permission is required or authentication is needed.
    ///
    public class AuthorizeException : Exception
    {
        readonly bool notoken;

        public AuthorizeException(bool notoken)
        {
            this.notoken = notoken;
        }

        public Nodule Nodoule { get; internal set; }

        public bool NoToken => notoken;
    }
}