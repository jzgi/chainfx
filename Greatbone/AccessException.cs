using System;

namespace Greatbone
{
    /// <summary>
    /// Thrown to indicate an failure of access check to a certain nodule. 
    /// </summary>
    public class AccessException : Exception
    {
        internal static readonly AccessException NoPrincipalEx = new AccessException(0, "no principal");

        internal static readonly AccessException NullResultEx = new AccessException(1, "access check with null result");

        internal static readonly AccessException FalseResultEx = new AccessException(2, "access check with false result");

        readonly int code;

        private AccessException(int code, string msg) : base(msg)
        {
            this.code = code;
        }

        /// <summary>
        /// No principal is presented.
        /// </summary>
        public bool NoPrincipal => code == 0;

        /// <summary>
        /// A principal is presented but a check operation returns null because of incompleteness of the principal.
        /// </summary>
        public bool NullResult => code == 1;

        /// <summary>
        /// A principal is presented but a check operation returns false.
        /// </summary>
        public bool FalseResult => code == 2;
    }
}