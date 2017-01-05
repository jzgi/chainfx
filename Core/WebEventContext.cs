using System;

namespace Greatbone.Core
{
    ///
    /// The processing of an queued message.
    ///
    public class WebEventContext : IDisposable
    {
        readonly WebClient client;

        long id;

        DateTime time;

        string name;

        string shard;

        // either JObj or JArr
        object entity;

        internal WebEventContext(WebClient client)
        {
            this.client = client;
        }

        internal void reset(long id, string name, string shard, DateTime time, object body)
        {
            this.id = id;
            this.name = name;
            this.shard = shard;
            this.time = time;
            this.entity = body;
        }

        public long Id => id;

        public string Name => name;

        public string Shard => shard;

        public ArraySegment<byte>? AsBytesSeg()
        {
            return entity as ArraySegment<byte>?;
        }

        public Form AsForm()
        {
            return entity as Form;
        }

        public JObj AsJObj()
        {
            return entity as JObj;
        }

        public JArr AsJArr()
        {
            return entity as JArr;
        }

        public D AsObject<D>(byte bits = 0) where D : IData, new()
        {
            ISource src = entity as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(bits);
        }

        public D[] AsArray<D>(byte bits = 0) where D : IData, new()
        {
            JArr jarr = entity as JArr;
            return jarr?.ToArray<D>(bits);
        }

        public XElem AsXElem()
        {
            return entity as XElem;
        }

        public void Cancel()
        {
            client.SetCancel();
        }

        public DbContext NewDbContext()
        {
            return null;
        }

        public void Dispose()
        {
        }
    }
}