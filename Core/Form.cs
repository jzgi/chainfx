using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A form object model parsed from either x-www-form-urlencoded or multipart/form-data.
    /// </summary>
    public class Form : Map<string, Field>, ISource
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
            int idx = IndexOf(name);
            if (idx == -1)
            {
                Add(new Field(name, v));
            }
            else
            {
                entries[idx].value.Add(v);
            }
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
        public bool Get<D>(string name, ref D v, byte proj = 0x0f) where D : IData, new()
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

        public ISource Let(out bool v)
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

        public ISource Let(out short v)
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

        public ISource Let(out int v)
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

        public ISource Let(out long v)
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

        public ISource Let(out double v)
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

        public ISource Let(out decimal v)
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

        public ISource Let(out DateTime v)
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

        public ISource Let(out string v)
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

        public ISource Let(out ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out short[] v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out int[] v)
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

        public ISource Let(out long[] v)
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

        public ISource Let(out string[] v)
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

        public ISource Let(out Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public ISource Let<D>(out D v, byte proj = 0x0f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public ISource Let<D>(out D[] v, byte proj = 0x0f) where D : IData, new()
        {
            throw new NotImplementedException();
        }


        public bool Get(string name, ref Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, byte proj = 0x0f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref ISource v)
        {
            throw new NotImplementedException();
        }

        public D ToObject<D>(byte proj = 0x0f) where D : IData, new()
        {
            D obj = new D();
            obj.Read(this, proj);
            return obj;
        }

        public D[] ToArray<D>(byte proj = 0x0f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void Write(ISink s)
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