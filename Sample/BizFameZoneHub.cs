using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
    public class BizFameZoneHub : WebZoneHub<Fame>
    {
        public BizFameZoneHub(WebServiceBuilder builder) : base(builder)
        {
            AddSub<BizFameZoneMgtSub>("mgt", null);
        }


        public override void Default(WebContext wc, Fame zone)
        {
            throw new System.NotImplementedException();
        }
    }
}