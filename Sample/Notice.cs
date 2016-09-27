using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Notice
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


        public void From(IInput r)
        {
            r.Get(nameof(id), ref id);
            r.Get(nameof(loc), ref loc);
            r.Get(nameof(authorid), ref authorid);
            r.Get(nameof(author), ref author);
            r.Get(nameof(date), ref date);
            r.Get(nameof(duedate), ref duedate);
            r.Get(nameof(subtype), ref subtype);
            r.Get(nameof(subject), ref subject);
            r.Get(nameof(remark), ref remark);
            r.Get(nameof(joins), ref joins);
        }

        public void To(IOutput r)
        {
            r.Put(nameof(id), id);
            r.Put(nameof(loc), loc);
            r.Put(nameof(authorid), authorid);
            r.Put(nameof(author), author);
            r.Put(nameof(date), date);
            r.Put(nameof(duedate), duedate);
            r.Put(nameof(subtype), subtype);
            r.Put(nameof(subject), subject);
            r.Put(nameof(remark), remark);
            r.Put(nameof(joins), joins);
        }
    }

    internal struct Join : IDat
    {
        internal char[] id;

        public void From(IInput r)
        {
            throw new NotImplementedException();
        }

        public void To(IOutput w)
        {
            throw new NotImplementedException();
        }
    }
}