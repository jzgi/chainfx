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

        // either JObj or JArr
        readonly object entity;

        internal WebEventContext(WebClient client, long id, string name, string shard, DateTime time, object body)
        {
            this.client = client;
            this.id = id;
            this.name = name;
            this.shard = shard;
            this.time = time;
            this.entity = body;
        }

        public long Id => id;

        public string Name => name;

        public string Shard => shard;

        public ArraySegment<byte>? GetBytesSegAsync()
        {
            return entity as ArraySegment<byte>?;
        }

        public Form GetForm()
        {
            return entity as Form;
        }

        public JObj GetJObj()
        {
            return entity as JObj;
        }

        public JArr GetJArr()
        {
            return entity as JArr;
        }

        public D GetObject<D>(byte bits = 0) where D : IData, new()
        {
            ISource src = entity as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(bits);
        }

        public D[] GetArray<D>(byte bits = 0) where D : IData, new()
        {
            JArr jarr = entity as JArr;
            return jarr?.ToArray<D>(bits);
        }

        public XElem GetXElem()
        {
            return entity as XElem;
        }

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