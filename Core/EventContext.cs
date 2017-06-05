﻿using System;
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

        public Work Work { get; set; }

        public EventInfo Doer { get; set; }

        public Service Service { get; set; }

        public byte[] Content => content;

        public M To<M>() where M : class, IDataInput
        {
            return entity as M;
        }

        public D ToData<D>(ushort proj = 0x00ff) where D : IData, new()
        {
            IDataInput inp = entity as IDataInput;
            if (inp == null)
            {
                return default(D);
            }
            return inp.ToData<D>(proj);
        }

        public D[] ToDatas<D>(ushort proj = 0x00ff) where D : IData, new()
        {
            IDataInput inp = entity as IDataInput;
            return inp?.ToDatas<D>(proj);
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