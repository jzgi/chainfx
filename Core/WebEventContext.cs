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

        public M As<M>() where M : class, IDataInput
        {
            return entity as M;
        }

        public D AsObject<D>(ushort proj = 0) where D : IData, new()
        {
            IDataInput src = entity as IDataInput;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(proj);
        }

        public D[] AsArray<D>(ushort proj = 0) where D : IData, new()
        {
            IDataInput srcs = entity as IDataInput;
            return srcs?.ToArray<D>(proj);
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