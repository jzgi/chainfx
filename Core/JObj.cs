using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A JSON object model.
    /// </summary>
    public class JObj : Map<string, JMbr>, IDataInput
    {
        public JObj(int capacity = 16) : base(capacity)
        {
        }

        /// To add null property
        public void Add(string name)
        {
            Add<JMbr>(new JMbr(JType.Null, name));
        }

        public void Add(string name, JObj v)
        {
            Add<JMbr>(new JMbr(v, name));
        }

        public void Add(string name, JArr v)
        {
            Add<JMbr>(new JMbr(v, name));
        }

        public void Add(string name, string v)
        {
            Add<JMbr>(new JMbr(v, name));
        }

        public void Add(string name, bool v)
        {
            Add<JMbr>(new JMbr(v, name));
        }

        public void Add(string name, JNumber v)
        {
            Add<JMbr>(new JMbr(v, name));
        }

        //
        // SOURCE
        //

        public bool Get(string name, ref bool v)
        {
            if (TryGet(name, out var mbr))
            {
                v = mbr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            if (TryGet(name, out var mbr))
            {
                v = mbr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            if (TryGet(name, out var mbr))
            {
                v = mbr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            if (TryGet(name, out var mbr))
            {
                v = mbr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            if (TryGet(name, out var mbr))
            {
                v = mbr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            if (TryGet(name, out var mbr))
            {
                v = mbr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            if (TryGet(name, out var mbr))
            {
                v = mbr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            if (TryGet(name, out var mbr))
            {
                v = mbr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            if (TryGet(name, out var mbr))
            {
                JArr ja = mbr;
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
            if (TryGet(name, out var mbr))
            {
                JArr ja = mbr;
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
            if (TryGet(name, out var mbr))
            {
                JArr ja = mbr;
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
            if (TryGet(name, out var mbr))
            {
                JArr ja = mbr;
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

        public bool Get(string name, ref Map<string, string> v)
        {
            if (TryGet(name, out var mbr))
            {
                if (mbr.type == JType.Object)
                {
                    JObj jo = mbr;
                    int count = jo.Count;
                    Map<string, string> map = new Map<string, string>();
                    for (int i = 0; i < count; i++)
                    {
                        JMbr e = jo[i];
                        map.Add(e.Key, e);
                    }
                    v = map;
                    return true;
                }
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, short proj = 0x00ff) where D : IData, new()
        {
            if (TryGet(name, out var mbr))
            {
                JObj jobj = mbr;
                if (jobj != null)
                {
                    v = new D();
                    v.Read(jobj);
                }
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D[] v, short proj = 0x00ff) where D : IData, new()
        {
            if (TryGet(name, out var mbr))
            {
                JArr ja = mbr;
                if (ja != null)
                {
                    v = new D[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        JObj jo = ja[i];
                        D dat = new D();
                        dat.Read(jo);
                        v[i] = dat;
                    }
                }
                return true;
            }
            return false;
        }


        public IDataInput Let(out bool v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out short v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out int v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out long v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out double v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out decimal v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out DateTime v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out string v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out short[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out int[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out long[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out string[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D v, short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D[] v, short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }


        public D ToObject<D>(short proj = 0x00ff) where D : IData, new()
        {
            D obj = new D();
            obj.Read(this, proj);
            return obj;
        }

        public D[] ToArray<D>(short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void Write<R>(IDataOutput<R> o) where R : IDataOutput<R>
        {
            for (int i = 0; i < Count; i++)
            {
                JMbr mbr = this[i];
                JType t = mbr.type;
                if (t == JType.Array)
                {
                    o.Put(mbr.Key, (IDataInput) (JArr) mbr);
                }
                else if (t == JType.Object)
                {
                    o.Put(mbr.Key, (JObj) mbr);
                }
                else if (t == JType.String)
                {
                    o.Put(mbr.Key, (string) mbr);
                }
                else if (t == JType.Number)
                {
                    o.Put(mbr.Key, (JNumber) mbr);
                }
                else if (t == JType.True)
                {
                    o.Put(mbr.Key, true);
                }
                else if (t == JType.False)
                {
                    o.Put(mbr.Key, false);
                }
                else if (t == JType.Null)
                {
                    o.PutNull(mbr.Key);
                }
            }
        }

        public DynamicContent Dump()
        {
            return new JsonContent(true).Put(null, this);
        }

        public bool DataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            JsonContent cont = new JsonContent(false, 4 * 1024);
            cont.Put(null, this);
            string str = cont.ToString();
            BufferUtility.Return(cont);
            return str;
        }
    }
}