using System;

namespace Greatbone
{
    /// <summary>
    /// Thrown to indicate an failure of authorization check to a certain nodule. A permission is required or authentication is needed.
    /// </summary>
    public class AuthorizeException : Exception
    {
        public static readonly AuthorizeException Null = new AuthorizeException(0, "principal is null");

        public static readonly AuthorizeException NotReady = new AuthorizeException(1, "principal is not ready");

        public static readonly AuthorizeException NotAllowed = new AuthorizeException(2, "principal is not allowed to access");

        readonly int code;

        private AuthorizeException(int code, string msg) : base(msg)
        {
            this.code = code;
        }

        public bool IsNull => code == 0;

        public bool IsNotReady => code == 1;

        public bool IsNotAllowed => code == 2;
    }
}