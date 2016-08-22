using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// <summary>The notice service.</summary>
	///
	public class NoticeService : WebService
	{
		public NoticeService(WebBuilder builder) : base(builder)
		{
			MountHub<NoticeXHub, int>(null);
		}
	}
}