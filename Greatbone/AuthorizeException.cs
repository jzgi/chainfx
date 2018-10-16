using System;

namespace Greatbone
{
    /// <summary>
    /// Thrown to indicate an failure of access check to a certain nodule. 
    /// </summary>
    public class AuthorizeException : Exception
    {
        readonly AuthorizeAttribute attribute;

        internal AuthorizeException(AuthorizeAttribute attribute = null, string msg = "access exception") : base(msg)
        {
            this.attribute = attribute;
        }

        public AuthorizeAttribute Attribute => attribute;
    }
}