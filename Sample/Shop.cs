using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// A shop data object.
    ///
    public class Shop : IForm
    {
        public static readonly Shop Empty = new Shop();

        internal string id; // platform shop id
        internal string name;
        internal string credential, password;
        internal string tel;
        internal double x, y;
        internal string prov;
        internal string city;
        internal string wx;
        internal string note;
        internal short status; // -1 dismissed, 0 closed, 1 open

        public void ReadData(IDataInput i, ushort proj = 0)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            if (proj.Kept())
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(x), ref x);
            i.Get(nameof(y), ref y);
            i.Get(nameof(prov), ref prov);
            i.Get(nameof(city), ref city);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(note), ref note);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            if (proj.Kept())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(tel), tel);
            o.Put(nameof(x), x);
            o.Put(nameof(y), y);
            o.Put(nameof(prov), prov);
            o.Put(nameof(city), city);
            o.Put(nameof(wx), wx);
            o.Put(nameof(note), note);
            o.Put(nameof(status), status);
        }

        public void WriteForm(HtmlContent h, ushort proj = 0)
        {
            throw new NotImplementedException();
        }

        public Token ToToken()
        {
            return new Token()
            {
                key = wx,
                name = name,
                role = 2
            };
        }
    }
}