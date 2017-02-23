using System;

namespace Greatbone.Core
{
    /// 
    /// To associate pre- and post- states to an action method.
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class StateAttribute : Attribute
    {
        public StateAttribute(int @if = 0, int unif = 0, int def = 0, int undef = 0)
        {
            If = @if;
            Unif = unif;
            Def = def;
            Undef = undef;
        }

        public int If { get; set; }

        public int Unif { get; set; }

        public int Def { get; set; }

        public int Undef { get; set; }
    }
}