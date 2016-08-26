using System.Threading;

namespace Greatbone.Core
{
	public class EqcClient
	{
		private Thread reconn;

		EqcConnection[] connections;

		public EqcClient()
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