using System;

namespace ChainBase.Web
{
    public class WebVarWork : WebWork
    {
        // to resolve from the principal object.
        public Func<IData, object> Accessor { get; internal set; }

        public object GetAccessor(IData prin)
        {
            return Accessor?.Invoke(prin);
        }
    }
}