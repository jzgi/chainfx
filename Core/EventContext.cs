using System;
using System.Data;

namespace Greatbone.Core
{
    ///
    /// The processing of an received web event. A single object is reused.
    ///
    public class EventContext : IDoerContext<EventInfo>, IDisposable
    {
        readonly Client client;

        internal long id;

        internal DateTime time;

        byte[] content;

        // either JObj or JArr
        object entity;

        internal EventContext(Client client)
        {
            this.client = client;
        }

        public long Id => id;

        public Folder Folder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public EventInfo Doer
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

        public ArraySegment<byte>? AsByteAs()
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
            IDataInput inp = entity as IDataInput;
            return inp?.ToArray<D>(proj);
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