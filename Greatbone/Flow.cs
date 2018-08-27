using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greatbone
{
    public delegate FlowContent FlowSource(long last);

    public delegate Task<FlowContent> FlowSourceAsync(long last);

    public delegate long FlowConsumer(FlowContext fc);

    /// <summary>
    /// The publishing of a data flow.
    /// </summary>
    public struct Publish
    {
        readonly string flow;

        readonly FlowSource _source;

        internal Publish(string flow,  FlowSource source)
        {
            this.flow = flow;
            this._source = source;
        }

        public string Flow => flow;

        public FlowSource Source => _source;
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