using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Notice : IData
    {
        internal int id;

        internal string loc;

        internal char[] authorid;

        internal string author;

        internal DateTime date;

        internal DateTime duedate;

        internal short subtype;

        internal string subject;

        internal string remark;

        internal List<Join> joins;


        public void In(IDataIn i)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(loc), ref loc);
            i.Get(nameof(authorid), ref authorid);
            i.Get(nameof(author), ref author);
            i.Get(nameof(date), ref date);
            i.Get(nameof(duedate), ref duedate);
            i.Get(nameof(subtype), ref subtype);
            i.Get(nameof(subject), ref subject);
            i.Get(nameof(remark), ref remark);
            i.Get(nameof(joins), ref joins);
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(loc), loc);
            o.Put(nameof(authorid), authorid);
            o.Put(nameof(author), author);
            o.Put(nameof(date), date);
            o.Put(nameof(duedate), duedate);
            o.Put(nameof(subtype), subtype);
            o.Put(nameof(subject), subject);
            o.Put(nameof(remark), remark);
            o.Put(nameof(joins), joins);
        }
    }

    internal struct Join : IData
    {
        internal char[] id;

        public void In(IDataIn i)
        {
            throw new NotImplementedException();
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            throw new NotImplementedException();
        }
    }
}