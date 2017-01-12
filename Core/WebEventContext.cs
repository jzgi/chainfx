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

        public D AsDat<D>(byte flags = 0) where D : IDat, new()
        {
            ISource src = entity as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToDat<D>(flags);
        }

        public D[] AsDats<D>(byte flags = 0) where D : IDat, new()
        {
            JArr jarr = entity as JArr;
            return jarr?.ToDats<D>(flags);
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