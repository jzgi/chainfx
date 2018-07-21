using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// A before filter that ensures a ctrid parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CtrAttribute : Attribute, IBefore
    {
        public bool Do(WebContext wc)
        {
            string ctrid = wc.Query[nameof(ctrid)];
            if (ctrid == null) // no cityid given then return a geolocator page 
            {
                var prin = (User) wc.Principal;
                if (prin?.ctrid == null)
                {
                    wc.GiveRedirect("/my//");
                    return false;
                }
                wc.AddParam(nameof(ctrid), prin.ctrid);
            }
            return true;
        }
    }
}