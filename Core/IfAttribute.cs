using System;

namespace Greatbone.Core
{
    public abstract class IfAttribute : Attribute
    {
        public abstract bool Check(WebContext wc, string sub);
    }
}