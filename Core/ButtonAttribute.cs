using System;

namespace Greatbone.Core
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ButtonAttribute : Attribute
    {

        public bool Dialog { get; set; }

    }
}