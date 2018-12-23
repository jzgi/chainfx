using System;
using Greatbone;

namespace Greatbone.Service
{
    /// <summary>
    /// Thrown to indicate an failure of access check to a certain nodule. 
    /// </summary>
    public class AuthorizeException : Exception
    {
        readonly Nodule target;

        readonly AuthorizeAttribute attribute;

        internal AuthorizeException(Nodule target, AuthorizeAttribute attribute = null, string msg = "authorize exception") : base(msg)
        {
            this.target = target;
            this.attribute = attribute;
        }

        /// <summary>
        /// The target work or action method.
        /// </summary>
        /// <seealso cref="Work"/>
        /// <seealso cref="Action"/>
        public Nodule Target => target;

        public AuthorizeAttribute Attribute => attribute;
    }
}