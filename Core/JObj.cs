using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON object model.
    ///
    public class JObj : Roll<JMember>, ISource
    {
        const int InitialCapacity = 16;

        public JObj(int capacity = InitialCapacity) : base(capacity)
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
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                byte[] bv = pair;
                v = new ArraySegment<byte>(bv);
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, byte bits = 0) where D : IData, new()
        {
            JMember pair;
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
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            JMember pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            JMember pair;
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
            JMember pair;
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
            JMember pair;
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
            JMember pair;
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

        public bool Get<D>(string name, ref D[] v, byte bits = 0) where D : IData, new()
        {
            JMember pair;
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
                JMember mbr = this[i];
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