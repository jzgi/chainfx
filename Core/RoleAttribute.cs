using System;

namespace Greatbone.Core
{
    ///
    /// An access check before the target nodule is invoked.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RoleAttribute : Attribute
    {
        public Nodule Nodule { get; internal set; }

        public bool Ui { get; set; }

        public bool Start { get; set; }

        public bool End { get; set; }

        public virtual void Aggregate() { }

        public virtual bool Check(ActionContext ac) => true;
    }
}