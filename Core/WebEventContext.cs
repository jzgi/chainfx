using System;

namespace Greatbone.Core
{
    ///
    /// The processing of an queued message. 
    ///
    public struct WebEventContext : IAutoContext, IDisposable
    {
        readonly WebReference reference;

        readonly long id;

        readonly string name;

        readonly string shard;

        // either Obj or Arr
        readonly object body;

        internal WebEventContext(WebReference reference, long id, string @event, string shard, object body)
        {
            this.reference = reference;
            this.id = id;
            this.name = @event;
            this.shard = shard;
            this.body = body;
        }

        public long Id => id;

        public string Name => name;

        public string Shard => shard;

        public Obj ToObj => (Obj)body;

        public Arr ToArr => (Arr)body;


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