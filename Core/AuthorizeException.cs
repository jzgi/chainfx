using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Thrown to indicate an failure of authorization check to a certain nodule. A permission is required or authentication is needed.
    /// </summary>
    public class AuthorizeException : Exception
    {
    }
}