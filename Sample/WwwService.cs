using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// The main website service (WWW).
	///
	public class WwwService : WebService
	{
		public WwwService(WebServiceContext wsc) : base(wsc)
		{

			AddSub<WwwNewsSub>("news", null);

			AddSub<WwwEventSub>("event", null);

			AddSub<WwwFameService>("fame", null);

			AddSub<WwwBrandService>("brand", null);
		}

		public void Show(WebContext wc)
		{
		}

		public void Contact(WebContext wc)
		{
		}
	}
}