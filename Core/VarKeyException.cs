using System;

namespace Greatbone.Core
{
    ///
    /// Thrown to indicate an failure of access check to a certain nodule. A permission is required or authentication is needed.
    ///
    public class VarKeyException : Exception
    {
        public VarKeyException() { }
    }
}