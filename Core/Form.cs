using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A form object model parsed from either x-www-form-urlencoded or multipart/form-data.
    ///
    public class Form : Roll<Field>, ISource, IReturnable
    {
        // if multipart
        readonly bool mp;

        public Form(bool mp, int capacity = 16) : base(capacity)
        {
            this.mp = mp;
        }

        public bool Data => mp;

        public void Add(string name, string v)
        {
            Add(new Field(name, v));
        }

        public void Add(string name, string filename, byte[] databuf, int count)
        {
            Add(new Field(name, filename, databuf, count));
        }

        public void Return()
        {
            if (Data)
            {
                for (int i = 0; i < Count; i++)
                {
                    byte[] buf = this[i].databuf;
                    if (buf != null)
                    {
                        BufferUtility.Return(buf);
                    }
                }
            }
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
            throw new NotImplementedException();
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

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            Field fld;
            if (TryGet(name, out fld))
            {
                v = fld;
                return true;
            }
            return false;
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

        public bool Get<D>(string name, ref D[] v, byte bits = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte bits = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }
    }
}