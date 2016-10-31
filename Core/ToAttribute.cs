using System;

namespace Greatbone.Core
{

    /// <summary>
    /// Test if the principal meets certain condition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ToAttribute : Attribute
    {

        public virtual bool Test(WebContext wc)
        {
            return true;
        }

    }

}