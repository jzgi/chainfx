using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A form object model parsed from either x-www-form-urlencoded or multipart/form-data.
    ///
    public class Form : Roll<Field>, IDataInput
    {
        // if multipart
        readonly bool mp;

        public Form(bool mp, int capacity = 16) : base(capacity)
        {
            this.mp = mp;
        }

        public bool Mp => mp;

        ///
        /// The backing buffer.
        ///
        public byte[] Buffer { get; internal set; }

        public void Add(string name, string v)
        {
            Add(new Field(name, v));
        }

        public void Add(string name, string filename, int offset, int count)
        {
            Add(new Field(name, filename, Buffer, offset, count));
        }

        //
        // SOURCE
        //

        public bool Get(string name, ref bool v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                string strv = fld;
                if (strv != null)
                {
                    JsonParse p = new JsonParse(strv);
                    v = p.Parse() as JArr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                string strv = fld;
                if (strv != null)
                {
                    JsonParse p = new JsonParse(strv);
                    v = p.Parse() as JObj;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D[] v, int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref List<D> v, int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D ToObject<D>(int proj = 0) where D : IData, new()
        {
            D dat = new D();
            dat.ReadData(this, proj);
            return dat;
        }

        public void WriteData<R>(IDataOutput<R> @out) where R : IDataOutput<R>
        {
            throw new NotImplementedException();
        }

        public IContent Dump()
        {
            var cont = new FormContent(true, true);
            cont.Put(null, this);
            return cont;
        }

        public bool DataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public D[] ToArray<D>(int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public List<D> ToList<D>(int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref IDataInput v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Dict v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref Map<D> v, int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }
    }
}