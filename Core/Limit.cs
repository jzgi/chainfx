using System;

namespace Greatbone.Core
{
    ///
    ///
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