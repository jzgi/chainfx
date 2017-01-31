using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON object model.
    ///
    public class JObj : Roll<JMem>, IModel, IDataInput
    {
        public JObj(int capacity = 16) : base(capacity)
        {
        }

        /// To add null property
        internal void AddNull(string name)
        {
            Add(new JMem(name));
        }

        internal void Add(string name, JObj v)
        {
            Add(new JMem(name, v));
        }

        internal void Add(string name, JArr v)
        {
            Add(new JMem(name, v));
        }

        internal void Add(string name, string v)
        {
            Add(new JMem(name, v));
        }

        internal void Add(string name, byte[] v)
        {
            Add(new JMem(name, v));
        }

        internal void Add(string name, bool v)
        {
            Add(new JMem(name, v));
        }

        internal void Add(string name, JNumber v)
        {
            Add(new JMem(name, v));
        }

        //
        // SOURCE
        //

        public bool Get(string name, ref bool v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                byte[] bv = mem;
                v = new ArraySegment<byte>(bv);
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, byte flags = 0) where D : IData, new()
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                JObj jobj = mem;
                if (jobj != null)
                {
                    v = new D();
                    v.ReadData(jobj);
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                JArr ja = mem;
                if (ja != null)
                {
                    v = new short[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        v[i] = ja[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                JArr ja = mem;
                if (ja != null)
                {
                    v = new int[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        v[i] = ja[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                JArr ja = mem;
                if (ja != null)
                {
                    v = new long[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        v[i] = ja[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                JArr ja = mem;
                if (ja != null)
                {
                    v = new string[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        v[i] = ja[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D[] v, byte flags = 0) where D : IData, new()
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                JArr ja = mem;
                if (ja != null)
                {
                    v = new D[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        JObj jo = ja[i];
                        D dat = new D();
                        dat.ReadData(jo);
                        v[i] = dat;
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref List<D> v, byte flags = 0) where D : IData, new()
        {
            JMem mem;
            if (TryGet(name, out mem))
            {
                JArr jarr = mem;
                if (jarr != null)
                {
                    v = new List<D>(jarr.Count + 8);
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        JObj jo = jarr[i];
                        D obj = new D();
                        obj.ReadData(jo);
                        v.Add(obj);
                    }
                }
                return true;
            }
            return false;
        }

        public bool DataSet => true;

        public void WriteData<R>(IDataOutput<R> dout) where R : IDataOutput<R>
        {
            for (int i = 0; i < Count; i++)
            {
                JMem mem = this[i];
                JType t = mem.type;
                if (t == JType.Array)
                {
                    dout.Put(mem.Key, (JArr)mem);
                }
                else if (t == JType.Object)
                {
                    dout.Put(mem.Key, (JObj)mem);
                }
                else if (t == JType.String)
                {
                    dout.Put(mem.Key, (string)mem);
                }
                else if (t == JType.Number)
                {
                    dout.Put(mem.Key, (JNumber)mem);
                }
                else if (t == JType.True)
                {
                    dout.Put(mem.Key, true);
                }
                else if (t == JType.False)
                {
                    dout.Put(mem.Key, false);
                }
                else if (t == JType.Null)
                {
                    dout.PutNull(mem.Key);
                }
            }
        }

        public override string ToString()
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, this);
            string str = cont.ToString();
            BufferUtility.Return(cont);
            return str;
        }

        public D ToObject<D>(byte flags = 0) where D : IData, new()
        {
            D obj = new D();
            obj.ReadData(this, flags);
            return obj;
        }

        public C Dump<C>() where C : IContent, IDataOutput<C>, new()
        {
            C cont = new C();
            WriteData(cont);
            return cont;
        }

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public D[] ToArray<D>(byte flags = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public List<D> ToList<D>(byte flags = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }
    }
}