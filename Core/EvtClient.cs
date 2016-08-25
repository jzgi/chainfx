using System.Threading;

namespace Greatbone.Core
{
	public class EvtClient
	{
		private Thread reconn;

		EvtConnection[] connections;

		public EvtClient()
		{
			reconn=new Thread(Reconnect);
		}

		void Reconnect()
		{
			foreach (var con in connections)
			{

			}
		}

		public void MakeConnections(string[] hosts)
		{

		}
	}
}