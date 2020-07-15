using System;
using SkyCloud.Web;

namespace SkyCloud
{
    /// <summary>
    /// A JSON object model.
    /// </summary>
    public class JObj : Map<string, JMbr>, ISource
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
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref char v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref short v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref int v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref long v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref double v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref string v)
        {
            if (TryGetValue(name, out var mbr))
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

        public bool Get(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            if (TryGetValue(name, out var mbr))
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
            if (TryGetValue(name, out var mbr))
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
            if (TryGetValue(name, out var mbr))
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
            if (TryGetValue(name, out var mbr))
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

        public bool Get(string name, ref JObj v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            if (TryGetValue(name, out var mbr))
            {
                v = mbr;
                return true;
            }

            return false;
        }

        public bool Get<D>(string name, ref D v, byte proj = 0x0f) where D : IData, new()
        {
            if (TryGetValue(name, out var mbr))
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

        public bool Get<D>(string name, ref D[] v, byte proj = 0x0f) where D : IData, new()
        {
            if (TryGetValue(name, out var mbr))
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

        public bool Next() => false;

        public void Write<C>(C cnt) where C : IContent, ISink
        {
            for (int i = 0; i < Count; i++)
            {
                var mbr = ValueAt(i);
                var t = mbr.type;
                if (t == JType.Array)
                {
                    cnt.Put(mbr.Key, (JArr) mbr);
                }
                else if (t == JType.Object)
                {
                    cnt.Put(mbr.Key, (JObj) mbr);
                }
                else if (t == JType.String)
                {
                    cnt.Put(mbr.Key, (string) mbr);
                }
                else if (t == JType.Number)
                {
                    cnt.Put(mbr.Key, (JNumber) mbr);
                }
                else if (t == JType.True)
                {
                    cnt.Put(mbr.Key, true);
                }
                else if (t == JType.False)
                {
                    cnt.Put(mbr.Key, false);
                }
                else if (t == JType.Null)
                {
                    cnt.PutNull(mbr.Key);
                }
            }
        }

        public IContent Dump()
        {
            var cnt = new JsonContent(4096);
            cnt.PutFromSource(this);
            return cnt;
        }

        public override string ToString()
        {
            var cnt = new JsonContent(4 * 1024, binary: false);
            try
            {
                cnt.Put(null, this);
                return cnt.ToString();
            }
            finally
            {
                WebUtility.Return(cnt.Buffer); // return buffer to pool
            }
        }
    }
}