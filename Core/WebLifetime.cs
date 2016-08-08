using System.Threading;
using Microsoft.AspNetCore.Hosting;

namespace Greatbone.Core
{
	public class WebLifetime : IApplicationLifetime
	{

		static readonly WebLifetime Lifetime = new WebLifetime();


		public void StopApplication()
		{
		}

		public CancellationToken ApplicationStarted { get; set; }

		public CancellationToken ApplicationStopping { get; set; }

		public CancellationToken ApplicationStopped { get; set; }
	}
}