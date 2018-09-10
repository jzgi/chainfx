using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greatbone
{
    /// <summary>
    /// The publishing of a data source.
    /// </summary>
    public struct Publish
    {
        readonly string key;

        readonly Func<long, DataContent> provide;

        readonly Func<long, Task<DataContent>> provideAsync;

        internal Publish(string key, Func<long, DataContent> provide)
        {
            this.key = key;
            this.provide = provide;
            this.provideAsync = null;
        }

        internal Publish(string key, Func<long, Task<DataContent>> provideAsync)
        {
            this.key = key;
            this.provide = null;
            this.provideAsync = provideAsync;
        }

        public string Key => key;

        public Func<long, DataContent> Provide => provide;
    }

    /// <summary>
    /// The subscribing of a data source.
    /// </summary>
    public struct Subscribe
    {
        readonly string keySpec;

        readonly string src;

        readonly Action<DataContext> consumer;

        // relevant http client connectors
        readonly List<Client> clients;

        public Subscribe(string keySpec, string src, Action<DataContext> consumer)
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