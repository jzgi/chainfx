using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Notice : ISerial
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


        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(id), ref id);
            r.Read(nameof(loc), ref loc);
            r.Read(nameof(authorid), ref authorid);
            r.Read(nameof(author), ref author);
            r.Read(nameof(date), ref date);
            r.Read(nameof(duedate), ref duedate);
            r.Read(nameof(subtype), ref subtype);
            r.Read(nameof(subject), ref subject);
            r.Read(nameof(remark), ref remark);
            r.Read(nameof(joins), ref joins);
        }

        public void WriteTo(ISerialWriter r)
        {
            r.Write(nameof(id), id);
            r.Write(nameof(loc), loc);
            r.Write(nameof(authorid), authorid);
            r.Write(nameof(author), author);
            r.Write(nameof(date), date);
            r.Write(nameof(duedate), duedate);
            r.Write(nameof(subtype), subtype);
            r.Write(nameof(subject), subject);
            r.Write(nameof(remark), remark);
            r.Write(nameof(joins), joins);
        }
    }

    internal struct Join : ISerial
    {
        internal char[] id;

        public void ReadFrom(ISerialReader r)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(ISerialWriter w)
        {
            throw new NotImplementedException();
        }
    }
}