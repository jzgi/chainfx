using System;

// ReSharper disable once CheckNamespace
namespace SkyCloud
{
    /// <summary>
    /// A form object model parsed from either x-www-form-urlencoded or multipart/form-data.
    /// </summary>
    public class Form : Map<string, Field>, ISource
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
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref char v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref short v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref int v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref long v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref double v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref string v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            if (TryGetValue(name, out var fld))
            {
                v = fld;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            return false;
        }

        public bool Get<D>(string name, ref D v, byte proj = 0x0f) where D : IData, new()
        {
            return false;
        }


        public bool Get<D>(string name, ref D[] v, byte proj = 0x0f) where D : IData, new()
        {
            return false;
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

        public bool IsDataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public void Write<C>(C cnt) where C : IContent, ISink
        {
            throw new NotImplementedException();
        }

        public IContent Dump()
        {
            var cnt = new FormContent(4096);
            cnt.Put(null, this);
            return cnt;
        }
    }
}