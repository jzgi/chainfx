using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON object model.
    ///
    public class JObj : Roll<JMem>, ISource
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
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                byte[] bv = pair;
                v = new ArraySegment<byte>(bv);
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, byte flags = 0) where D : IData, new()
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                JObj jobj = pair;
                if (jobj != null)
                {
                    v = new D();
                    v.Load(jobj);
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                JArr jarr = pair;
                if (jarr != null)
                {
                    v = new short[jarr.Count];
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        v[i] = jarr[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                JArr jarr = pair;
                if (jarr != null)
                {
                    v = new int[jarr.Count];
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        v[i] = jarr[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                JArr jarr = pair;
                if (jarr != null)
                {
                    v = new long[jarr.Count];
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        v[i] = jarr[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                JArr jarr = pair;
                if (jarr != null)
                {
                    v = new string[jarr.Count];
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        v[i] = jarr[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D[] v, byte flags = 0) where D : IData, new()
        {
            JMem pair;
            if (TryGet(name, out pair))
            {
                JArr jarr = pair;
                if (jarr != null)
                {
                    v = new D[jarr.Count];
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        JObj jobj = jarr[i];
                        D dat = new D();
                        dat.Load(jobj);
                        v[i] = dat;
                    }
                }
                return true;
            }
            return false;
        }


        internal void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < Count; i++)
            {
                JMem mbr = this[i];
                JType typ = mbr.type;
                if (typ == JType.Array)
                {
                    snk.Put(mbr.Name, (JArr) mbr);
                }
                else if (typ == JType.Object)
                {
                    snk.Put(mbr.Name, (JObj) mbr);
                }
                else if (typ == JType.String)
                {
                    snk.Put(mbr.Name, (string) mbr);
                }
                else if (typ == JType.Number)
                {
                    snk.Put(mbr.Name, (JNumber) mbr);
                }
                else if (typ == JType.True)
                {
                    snk.Put(mbr.Name, true);
                }
                else if (typ == JType.False)
                {
                    snk.Put(mbr.Name, false);
                }
                else if (typ == JType.Null)
                {
                    snk.PutNull(mbr.Name);
                }
            }
        }

        public override string ToString()
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Add('{');
            Dump(cont);
            cont.Add('}');
            string str = cont.ToString();
            BufferUtility.Return(cont);
            return str;
        }
    }
}