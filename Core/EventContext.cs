using System;
using System.Data;

namespace Greatbone.Core
{
    /// <summary>
    /// The processing of an received distributed event. A single instance can be reused.
    /// </summary>
    public class EventContext : IDoerContext<EventInfo>, IDisposable
    {
        readonly Client client;

        internal long id;

//        internal DateTime time;

        byte[] content;

        // either JObj or JArr
        object entity;

        internal EventContext(Client client)
        {
            this.client = client;
        }

        public long Id => id;

        public Service Service { get; internal set; }

        public Work Work { get; internal set; }

        public EventInfo Doer { get; internal set; }

        public int Subscript { get; internal set; }

        public byte[] Content => content;

        public M To<M>() where M : class, IDataInput
        {
            return entity as M;
        }

        public D ToObject<D>(short proj = 0x00ff) where D : IData, new()
        {
            if (!(entity is IDataInput inp))
            {
                return default;
            }
            return inp.ToObject<D>(proj);
        }

        public D[] ToArray<D>(short proj = 0x00ff) where D : IData, new()
        {
            IDataInput inp = entity as IDataInput;
            return inp?.ToArray<D>(proj);
        }


        DbContext dbctx;

        public DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbctx == null)
            {
                DbContext dc = new DbContext(Service, this);
                if (level != null)
                {
                    dc.Begin(level.Value);
                }
                dbctx = dc;
            }
            return dbctx;
        }

        public void Dispose()
        {
        }
    }
}