using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// <summary>The notice service.</summary>
	///
	public class NoticeService : WebService
	{
		public NoticeService(WebServiceContext wsc) : base(wsc)
		{
			MountHub<NoticeXHub>(false);
		}
	}
}