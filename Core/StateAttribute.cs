using System;

namespace Greatbone.Core
{
    /// 
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class StateAttribute : Attribute
    {
        public StateAttribute(int @if = 0, int unif = 0, int def = 0, int undef = 0)
        {
            Unif = unif;
            If = @if;
            Def = def;
            Undef = undef;
        }

        public int If { get; set; } = 0;

        public int Unif { get; set; } = 0;

        public int Def { get; set; }

        public int Undef { get; set; }
    }
}