using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A form object model parsed from x-www-form-urlencoded.
    ///
    public class Form : Roll<Pair>, ISource
    {
        const int InitialCapacity = 8;

        public static readonly Form Empty = new Form();

        public Form(int capacity = InitialCapacity) : base(capacity)
        {
        }

        internal void Add(string name, string v)
        {
            Add(new Pair(name, v));
        }

        //
        // SOURCE
        //

        public bool Get(string name, ref bool v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JNumber v)
        {
            throw new NotImplementedException();
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
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
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

        public bool Get(string name, ref short[] v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            Pair pr;
            if (TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
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
    }
}