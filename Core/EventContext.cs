using System;
using System.Data;

namespace Greatbone.Core
{
    ///
    /// The processing of an received web event. A single object is reused.
    ///
    public class EventContext : IHandlerContext<EventInfo>, IDisposable
    {
        readonly Client client;

        internal long id;

        internal string name;

        internal string shard;

        internal string arg;

        byte[] content;

        // either JObj or JArr
        object entity;

        internal EventContext(Client client)
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

        public Folder Folder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public EventInfo Handler
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Service Service
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ArraySegment<byte>? AsBytesSeg()
        {
            return entity as ArraySegment<byte>?;
        }

        public M As<M>() where M : class, IDataInput
        {
            return entity as M;
        }

        public D AsObject<D>(int proj = 0) where D : IData, new()
        {
            IDataInput src = entity as IDataInput;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(proj);
        }

        public D[] AsArray<D>(int proj = 0) where D : IData, new()
        {
            IDataInput srcs = entity as IDataInput;
            return srcs?.ToArray<D>(proj);
        }

        public void Cancel()
        {
            client.SetCancel();
        }

        public DbContext NewDbContext(IsolationLevel? level = null)
        {
            return null;
        }

        public void Dispose()
        {
        }
    }
}