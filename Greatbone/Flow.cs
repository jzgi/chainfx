using System.Collections.Generic;

namespace Greatbone
{
    public delegate FlowContent FlowProvider(long last);

    public delegate long FlowConsumer(FlowContext fc);

    /// <summary>
    /// The publishing of a data flow.
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
    /// The subscribing of a data flow.
    /// </summary>
    public struct Subscribe
    {
        readonly string keySpec;

        readonly string flow;

        readonly FlowConsumer consumer;

        // relevant http client connectors
        readonly List<Client> clients;

        public Subscribe(string keySpec, string flow, FlowConsumer consumer)
        {
            this.keySpec = keySpec;
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