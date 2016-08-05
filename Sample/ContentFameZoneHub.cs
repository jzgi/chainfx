using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
    public class ContentFameZoneHub : WebZoneHub<Fame>
    {
        public ContentFameZoneHub(WebServiceContext wsc) : base(wsc)
        {
            AddSub<ContentFameZoneMgtSub>("mgt", null);
        }


        public override void Default(WebContext wc, Fame zone)
        {
            throw new System.NotImplementedException();
        }
    }
}