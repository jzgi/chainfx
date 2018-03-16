using System;

namespace Greatbone
{
    /// <summary>
    /// To specify page size thru annotating on the page parameter in a procedure method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class LimitAttribute : Attribute
    {
        readonly int value;

        public LimitAttribute(int value = 20)
        {
            this.value = value;
        }

        public int Value => value;
    }
}