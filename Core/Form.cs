using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A form object model parsed from either x-www-form-urlencoded or multipart/form-data.
    /// </summary>
    public class Form : Roll<Field>, IDataInput
    {
        // if multipart
        readonly bool mp;

        int ordinal;

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
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

//        public bool Get(string name, ref ArraySegment<byte> v)
//        {
//            Field fld;
//            if (TryGet(name, out fld))
//            {
//                v = fld;
//                return true;
//            }
//            return false;
//        }
//
        public bool Get<D>(string name, ref D v, short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            if (TryGet(name, out var fld))
            {
                v = fld;
                return true;
            }
            return false;
        }

        //
        // LET
        //

        public IDataInput Let(out bool v)
        {
            int ord = ordinal++;
            if (ord < Count)
            {
                v = this[ord];
                return this;
            }
            v = false;
            return this;
        }

        public IDataInput Let(out short v)
        {
            int ord = ordinal++;
            if (ord < Count)
            {
                v = this[ord];
                return this;
            }
            v = 0;
            return this;
        }

        public IDataInput Let(out int v)
        {
            int ord = ordinal++;
            if (ord < Count)
            {
                v = this[ord];
                return this;
            }
            v = 0;
            return this;
        }

        public IDataInput Let(out long v)
        {
            int ord = ordinal++;
            if (ord < Count)
            {
                v = this[ord];
                return this;
            }
            v = 0;
            return this;
        }

        public IDataInput Let(out double v)
        {
            int ord = ordinal++;
            if (ord < Count)
            {
                v = this[ord];
                return this;
            }
            v = 0;
            return this;
        }

        public IDataInput Let(out decimal v)
        {
            int ord = ordinal++;
            if (ord < Count)
            {
                v = this[ord];
                return this;
            }
            v = 0;
            return this;
        }

        public IDataInput Let(out DateTime v)
        {
            int ord = ordinal++;
            if (ord < Count)
            {
                v = this[ord];
                return this;
            }
            v = default;
            return this;
        }

        public IDataInput Let(out string v)
        {
            int ord = ordinal++;
            if (ord < Count)
            {
                v = this[ord];
                return this;
            }
            v = null;
            return this;
        }

        public IDataInput Let(out ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out short[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out int[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out long[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out string[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out Dictionary<string, string> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D v, short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D[] v, short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }


        public bool Get(string name, ref Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref IDataInput v)
        {
            throw new NotImplementedException();
        }

        public D ToObject<D>(short proj = 0x00ff) where D : IData, new()
        {
            D obj = new D();
            obj.Read(this, proj);
            return obj;
        }

        public D[] ToArray<D>(short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void Write<R>(IDataOutput<R> @out) where R : IDataOutput<R>
        {
            throw new NotImplementedException();
        }

        public DynamicContent Dump()
        {
            var cont = new FormContent(true);
            cont.Put(null, this);
            return cont;
        }

        public bool DataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }
    }
}