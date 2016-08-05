using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
    public class WwwFameZoneHub : WebZoneHub<Fame>
    {
        public WwwFameZoneHub(WebServiceContext wsc) : base(wsc)
        {
            AddSub<WwwFameZoneMgtSub>("mgt", null);
        }


        public override void Default(WebContext wc, Fame zone)
        {
            throw new System.NotImplementedException();
        }
    }
}