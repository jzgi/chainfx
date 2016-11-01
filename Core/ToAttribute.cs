using System;

namespace Greatbone.Core
{

    /// <summary>
    /// Test if the principal meets certain condition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ToAttribute : Attribute
    {

        readonly bool bearer;

        public ToAttribute(bool bearer = true)
        {
            this.bearer = bearer;
        }

        public virtual bool Test(WebContext wc)
        {
            return true;
        }

        public bool IsBearer => bearer;

    }

}