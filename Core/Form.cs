using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A form object model parsed from x-www-form-urlencoded.
    ///
    public class Form : Roll<Field>, ISource, IReturnable
    {
        public static readonly Form Empty = new Form();

        public Form(int capacity = 16) : base(capacity)
        {
        }

        internal void Add(string name, string v)
        {
            Add(new Field(name, v));
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

        public bool Get(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
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

        public bool Get(string name, ref char[] v)
        {
            throw new NotImplementedException();
        }


        public bool Get<D>(string name, ref D[] v, byte bits = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte bits = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void Return()
        {
            throw new NotImplementedException();
        }
    }
}