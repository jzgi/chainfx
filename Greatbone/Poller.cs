using System;
using System.Collections.Generic;

namespace Greatbone
{
    /// <summary>
    /// The subscribing of a data source.
    /// </summary>
    public class Poller
    {
        readonly string key;

        readonly Action<WebContext> consumer;

        // relevant http client connectors
        readonly List<Client> refs;

        public Poller(string key, Action<WebContext> consumer)
        {
            this.key = key;
            this.consumer = consumer;
            this.refs = new List<Client>(4);
        }

        public List<Client> Refs => refs;

        internal void TryPoll(int tick)
        {
        }
    }
}