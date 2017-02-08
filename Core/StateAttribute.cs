using System;

namespace Greatbone.Core
{
    /// 
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class StateAttribute : Attribute
    {
        public StateAttribute(int with, int without, int set)
        {
            Without = without;
            With = with;
            Set = set;
        }

        public int With { get; set; } = 0;

        public int Without { get; set; } = 0;

        public int Set { get; set; }

        public int Unset { get; set; }
    }
}