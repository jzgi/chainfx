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

        string name;

        // either JObj or JArr
        object entity;

        internal WebEventContext(WebClient client)
        {
            this.client = client;
        }

        internal void Reset(long id, string name, DateTime time, string mtype, object body)
        {
            this.id = id;
            this.name = name;
            this.entity = body;
        }

        public long Id => id;

        public string Name => name;

        public ArraySegment<byte>? AsBytesSeg()
        {
            return entity as ArraySegment<byte>?;
        }

        public M As<M>() where M : class, IModel
        {
            return entity as M;
        }

        public D AsObject<D>(byte flags = 0) where D : IData, new()
        {
            ISource src = entity as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(flags);
        }

        public D[] AsArray<D>(byte flags = 0) where D : IData, new()
        {
            ISourceSet srcs = entity as ISourceSet;
            return srcs?.ToArray<D>(flags);
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