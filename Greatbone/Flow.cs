using System.Collections.Generic;

namespace Greatbone
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pack"></param>
    /// <returns>the last eventid that has been received and handled</returns>
    public delegate FlowContent FlowProvider(long last);

    public delegate long FlowConsumer(FlowContext fc);

    /// <summary>
    /// The publishing of data revision.
    /// </summary>
    public struct Publish
    {
        readonly string flow;

        readonly FlowProvider provider;

        internal Publish(string flow, FlowProvider provider)
        {
            this.flow = flow;
            this.provider = provider;
        }

        public string Flow => flow;

        public FlowProvider Provider => provider;
    }

    /// <summary>
    /// The subscribingof a particular type of data events.
    /// </summary>
    public struct Subscribe
    {
        readonly string svcspec;

        readonly string flow;

        readonly FlowConsumer consumer;

        // relevant http client connectors
        readonly List<Client> clients;

        public Subscribe(string svcspec, string flow, FlowConsumer consumer)
        {
            this.svcspec = svcspec;
            this.flow = flow;
            this.consumer = consumer;
            this.clients = new List<Client>(4);
        }

        public List<Client> Clients => clients;

        internal void TryPoll(int tick)
        {
        }
    }
}