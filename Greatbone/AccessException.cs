using System;

namespace Greatbone
{
    /// <summary>
    /// Thrown to indicate an failure of access check to a certain nodule. 
    /// </summary>
    public class AccessException : Exception
    {
        readonly AccessAttribute attribute;

        internal AccessException(AccessAttribute attribute = null, string msg = "access exception") : base(msg)
        {
            this.attribute = attribute;
        }

        public AccessAttribute Attribute => attribute;
    }
}