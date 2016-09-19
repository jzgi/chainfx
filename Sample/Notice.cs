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
            r.Read(nameof(id), out id);
            r.Read(nameof(loc), out loc);
            r.Read(nameof(authorid), out authorid);
            r.Read(nameof(author), out author);
            r.Read(nameof(date), out date);
            r.Read(nameof(duedate), out duedate);
            r.Read(nameof(subtype), out subtype);
            r.Read(nameof(subject), out subject);
            r.Read(nameof(remark), out remark);
            r.Read(nameof(joins), out joins);
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