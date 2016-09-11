using System.Threading;

namespace Greatbone.Core
{
    public class MsgClient
    {
        private Thread reconn;

        MsgConnection[] connections;

        public MsgClient()
        {
            reconn = new Thread(Reconnect);
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