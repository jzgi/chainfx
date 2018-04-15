using System;

namespace Greatbone
{
    /// <summary>
    /// Thrown to indicate an failure of authorization check to a certain nodule. 
    /// </summary>
    public class AuthorizeException : Exception
    {
        public static readonly AuthorizeException NoPrincipalEx = new AuthorizeException(0, "no principal");

        public static readonly AuthorizeException NullResultEx = new AuthorizeException(1, "authorize null result");

        public static readonly AuthorizeException FalseResultEx = new AuthorizeException(2, "authorize false result");

        readonly int code;

        private AuthorizeException(int code, string msg) : base(msg)
        {
            this.code = code;
        }

        public bool NoPrincipal => code == 0;

        public bool NullResult => code == 1;

        public bool FalseResult => code == 2;
    }
}