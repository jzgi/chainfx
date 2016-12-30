using System;

namespace Greatbone.Core
{
    ///
    /// The processing of an queued message. 
    ///
    public struct WebEventContext : IDisposable
    {
        readonly WebClient client;

        readonly long id;

        readonly DateTime time;

        readonly string name;

        readonly string shard;

        // either Obj or Arr
        readonly object body;

        internal WebEventContext(WebClient client, long id, string name, string shard, DateTime time, object body)
        {
            this.client = client;
            this.id = id;
            this.name = name;
            this.shard = shard;
            this.time = time;
            this.body = body;
        }

        public long Id => id;

        public string Name => name;

        public string Shard => shard;

        public JObj ToObj => (JObj)body;

        public JArr ToArr => (JArr)body;


        public DbContext NewDbContext()
        {
            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}