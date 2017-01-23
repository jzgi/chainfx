using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON object model.
    ///
    public class JObj : Roll<JMem>, IModel, ISource
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
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                byte[] bv = prop;
                v = new ArraySegment<byte>(bv);
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, byte flags = 0) where D : IData, new()
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                JObj jobj = prop;
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
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                v = prop;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                JArr jarr = prop;
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
            JMem prop;
            if (TryGet(name, out prop))
            {
                JArr jarr = prop;
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
            JMem prop;
            if (TryGet(name, out prop))
            {
                JArr jarr = prop;
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
            JMem prop;
            if (TryGet(name, out prop))
            {
                JArr jarr = prop;
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
            JMem prop;
            if (TryGet(name, out prop))
            {
                JArr jarr = prop;
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

        public bool Get<D>(string name, ref List<D> v, byte flags = 0) where D : IData, new()
        {
            JMem prop;
            if (TryGet(name, out prop))
            {
                JArr jarr = prop;
                if (jarr != null)
                {
                    v = new List<D>(jarr.Count + 8);
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        JObj jobj = jarr[i];
                        D dat = new D();
                        dat.Load(jobj);
                        v.Add(dat);
                    }
                }
                return true;
            }
            return false;
        }

        public void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < Count; i++)
            {
                JMem prop = this[i];
                JType typ = prop.type;
                if (typ == JType.Array)
                {
                    snk.Put(prop.Name, (JArr)prop);
                }
                else if (typ == JType.Object)
                {
                    snk.Put(prop.Name, (JObj)prop);
                }
                else if (typ == JType.String)
                {
                    snk.Put(prop.Name, (string)prop);
                }
                else if (typ == JType.Number)
                {
                    snk.Put(prop.Name, (JNumber)prop);
                }
                else if (typ == JType.True)
                {
                    snk.Put(prop.Name, true);
                }
                else if (typ == JType.False)
                {
                    snk.Put(prop.Name, false);
                }
                else if (typ == JType.Null)
                {
                    snk.PutNull(prop.Name);
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

        public D ToObject<D>(byte flags = 0) where D : IData, new()
        {
            D dat = new D();
            dat.Load(this, flags);
            return dat;
        }

        public IContent Dump()
        {
            JsonContent cont = new JsonContent();
            Dump(cont);
            return cont;
        }
    }
}