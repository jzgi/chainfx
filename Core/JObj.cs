using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON object model.
    ///
    public class JObj : Roll<JMember>, IDataInput
    {
        public JObj(int capacity = 16) : base(capacity)
        {
        }

        /// To add null property
        internal void AddNull(string name)
        {
            Add(new JMember(name));
        }

        internal void Add(string name, JObj v)
        {
            Add(new JMember(name, v));
        }

        internal void Add(string name, JArr v)
        {
            Add(new JMember(name, v));
        }

        internal void Add(string name, string v)
        {
            Add(new JMember(name, v));
        }

        internal void Add(string name, byte[] v)
        {
            Add(new JMember(name, v));
        }

        internal void Add(string name, bool v)
        {
            Add(new JMember(name, v));
        }

        internal void Add(string name, JNumber v)
        {
            Add(new JMember(name, v));
        }

        //
        // SOURCE
        //

        public bool Get(string name, ref bool v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                v = mem;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                byte[] bv = mem;
                v = new ArraySegment<byte>(bv);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            JMember mem;
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
            JMember mem;
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
            JMember mem;
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
            JMember mem;
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

        public bool Get(string name, ref Dictionary<string, string> v)
        {
            JMember mem;
            if (TryGet(name, out mem))
            {
                if (mem.type == JType.Object)
                {
                    JObj jo = mem;
                    int count = jo.Count;
                    Dictionary<string, string> dict = new Dictionary<string, string>(count);
                    for (int i = 0; i < count; i++)
                    {
                        JMember e = jo[i];
                        dict.Add(e.Name, (string)e);
                    }
                    v = dict;
                    return true;
                }
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, ushort proj = 0) where D : IData, new()
        {
            JMember mem;
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

        public bool Get<D>(string name, ref D[] v, ushort proj = 0) where D : IData, new()
        {
            JMember mem;
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

        public bool Get<D>(string name, ref List<D> v, ushort proj = 0) where D : IData, new()
        {
            JMember mem;
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

        public D ToObject<D>(ushort proj = 0) where D : IData, new()
        {
            D obj = new D();
            obj.ReadData(this, proj);
            return obj;
        }

        public D[] ToArray<D>(ushort proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public List<D> ToList<D>(ushort proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void WriteData<R>(IDataOutput<R> dout) where R : IDataOutput<R>
        {
            for (int i = 0; i < Count; i++)
            {
                JMember mem = this[i];
                JType t = mem.type;
                if (t == JType.Array)
                {
                    dout.Put(mem.Name, (JArr)mem);
                }
                else if (t == JType.Object)
                {
                    dout.Put(mem.Name, (JObj)mem);
                }
                else if (t == JType.String)
                {
                    dout.Put(mem.Name, (string)mem);
                }
                else if (t == JType.Number)
                {
                    dout.Put(mem.Name, (JNumber)mem);
                }
                else if (t == JType.True)
                {
                    dout.Put(mem.Name, true);
                }
                else if (t == JType.False)
                {
                    dout.Put(mem.Name, false);
                }
                else if (t == JType.Null)
                {
                    dout.PutNull(mem.Name);
                }
            }
        }

        public C Dump<C>() where C : IContent, IDataOutput<C>, new()
        {
            C cont = new C();
            WriteData(cont);
            return cont;
        }

        public bool DataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, this);
            string str = cont.ToString();
            BufferUtility.Return(cont);
            return str;
        }
    }
}