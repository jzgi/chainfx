using System;

namespace Greatbone.Core
{

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DialogAttribute : Attribute
    {
        readonly int size;

        public DialogAttribute(int size = 0)
        {
            this.size = size;
        }

    }

}