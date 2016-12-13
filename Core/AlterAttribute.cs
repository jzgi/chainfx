using System;

namespace Greatbone.Core
{

    ///
    /// Working on a directory or individual action, to manipulate the action context as well as the result returned from it.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AlterAttribute : Attribute
    {

        public abstract void PreDo(WebActionContext ac);

        public abstract void PostDo(WebActionContext ac);
    }
}