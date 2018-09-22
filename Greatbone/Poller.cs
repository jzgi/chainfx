using System;
using System.Collections.Generic;

namespace Greatbone
{
    /// <summary>
    /// The subscribing of a data source.
    /// </summary>
    public class Poller
    {
        readonly string keySpec;

        readonly string src;

        readonly Action<WebContext> consumer;

        // relevant http client connectors
        readonly List<Client> clients;

        public Poller(string keySpec, string src, Action<WebContext> consumer)
        {
            this.keySpec = keySpec;
            this.src = src;
            this.consumer = consumer;
            this.clients = new List<Client>(4);
        }

        public List<Client> Clients => clients;

        internal void TryPoll(int tick)
        {
        }
    }
}