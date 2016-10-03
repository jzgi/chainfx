using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Notice : IPersist
    {
        internal int id;

        internal string loc;

        internal string authorid;

        internal string author;

        internal DateTime date;

        internal DateTime duedate;

        internal short subtype;

        internal string subject;

        internal string remark;

        internal List<Join> joins;


        public void Load(ISource sc, int x)
        {
            sc.Get(nameof(id), ref id);
            sc.Get(nameof(loc), ref loc);
            sc.Get(nameof(authorid), ref authorid);
            sc.Get(nameof(author), ref author);
            sc.Get(nameof(date), ref date);
            sc.Get(nameof(duedate), ref duedate);
            sc.Get(nameof(subtype), ref subtype);
            sc.Get(nameof(subject), ref subject);
            sc.Get(nameof(remark), ref remark);
            sc.Get(nameof(joins), ref joins);
        }

        public void Save<R>(ISink<R> sk, int x) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(loc), loc);
            sk.Put(nameof(authorid), authorid);
            sk.Put(nameof(author), author);
            sk.Put(nameof(date), date);
            sk.Put(nameof(duedate), duedate);
            sk.Put(nameof(subtype), subtype);
            sk.Put(nameof(subject), subject);
            sk.Put(nameof(remark), remark);
            sk.Put(nameof(joins), joins);
        }
    }

    internal struct Join : IPersist
    {
        internal char[] id;

        public void Load(ISource sc, int x)
        {
            throw new NotImplementedException();
        }

        public void Save<R>(ISink<R> sk, int x) where R : ISink<R>
        {
            throw new NotImplementedException();
        }
    }
}