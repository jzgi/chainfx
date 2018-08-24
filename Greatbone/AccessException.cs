using System;

namespace Greatbone
{
    /// <summary>
    /// Thrown to indicate an failure of access check to a certain nodule. 
    /// </summary>
    public class AccessException : Exception
    {
        internal static readonly AccessException NoPrincipalEx = new AccessException(null, null, "must have principal");

        internal static readonly AccessException FalseResultEx = new AccessException(false, null, "no access");


        readonly bool? result;

        readonly AccessAttribute attribute;

        internal AccessException(bool? result, AccessAttribute attribute = null, string msg = "access exception") : base(msg)
        {
            this.attribute = attribute;
            this.result = result;
        }

        public bool? Result => result;

        public AccessAttribute Attribute => attribute;
    }
}