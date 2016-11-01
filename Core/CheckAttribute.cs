using System;

namespace Greatbone.Core
{

    /// <summary>
    /// Test if the principal meets certain condition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckAttribute : Attribute
    {

        readonly bool bearer;

        public CheckAttribute(bool bearer = true)
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