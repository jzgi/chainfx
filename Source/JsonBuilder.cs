using System;
using System.Collections.Generic;

namespace ChainFx
{
    ///
    /// To generate a UTF-8 encoded JSON document. An extension of putting byte array is supported.
    ///
    public class JsonBuilder : ContentBuilder, ISink
    {
        private const int MAX_LEVELS = 16;

        // starting positions of each level
        private readonly int[] counts;

        // current level
        private int level;

        public JsonBuilder(bool bytely, int capacity) : base(bytely, capacity)
        {
            counts = new int[MAX_LEVELS];
            level = 0;
        }

        public override string CType { get; set; } = "application/json; charset=utf-8";

        private void AddEsc(string v)
        {
            if (v == null) return;

            foreach (var c in v)
            {
                if (c == '\"')
                {
                    Add('\\');
                    Add('"');
                }
                else if (c == '\\')
                {
                    Add('\\');
                    Add('\\');
                }
                else if (c == '\n')
                {
                    Add('\\');
                    Add('n');
                }
                else if (c == '\r')
                {
                    Add('\\');
                    Add('r');
                }
                else if (c == '\t')
                {
                    Add('\\');
                    Add('t');
                }
                else
                {
                    Add(c);
                }
            }
        }

        public void ARR(Action<JsonBuilder> a)
        {
            ARR_();
            a?.Invoke(this);
            _ARR();
        }

        public JsonBuilder ARR_(string name = null)
        {
            if (counts[level]++ > 0) Add(',');
            counts[++level] = 0; // enter
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add('[');
            return this;
        }

        public JsonBuilder _ARR()
        {
            Add(']');
            level--; // exit
            return this;
        }

        public void OBJ(Action<JsonBuilder> a)
        {
            OBJ_();
            a?.Invoke(this);
            _OBJ();
        }

        public JsonBuilder OBJ_(string name = null)
        {
            if (counts[level]++ > 0) Add(',');
            counts[++level] = 0; // enter
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add('{');
            return this;
        }

        public JsonBuilder _OBJ()
        {
            Add('}');
            level--; // exit
            return this;
        }

        //
        // SINK
        //
        public void PutNull(string name)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add("null");
        }

        public void Put(string name, JNumber v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add(v.bigint);
            if (v.Pt)
            {
                Add('.');
                Add(v.fract);
            }
        }

        public void Put(string name, bool v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(v ? "true" : "false");
        }

        public void Put(string name, char v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, short v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(v);
        }

        public void Put(string name, int v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(v);
        }

        public void Put(string name, long v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(v);
        }

        public void Put(string name, double v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(v);
        }

        public void Put(string name, decimal v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(v);
        }

        public void Put(string name, DateTime v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, TimeSpan v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, string v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('"');
                AddEsc(v);
                Add('"');
            }
        }

        public void Put(string name, ArraySegment<byte> v)
        {
        }

        public void Put(string name, byte[] v)
        {
        }

        public void Put(string name, short[] v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(v[i]);
                }
                Add(']');
            }
        }

        public void Put(string name, int[] v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(v[i]);
                }
                Add(']');
            }
        }

        public void Put(string name, long[] v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(v[i]);
                }
                Add(']');
            }
        }

        public void Put(string name, string[] v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(',');
                    string str = v[i];
                    if (str == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        Add('"');
                        AddEsc(str);
                        Add('"');
                    }
                }
                Add(']');
            }
        }


        public void Put(string name, JObj v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                Add('{');
                v.Write(this);
                Add('}');
                level--; // exit
            }
        }

        public void Put(string name, JArr v)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                Add('[');
                v.Write(this);
                Add(']');
                level--; // exit
            }
        }

        public void Put(string name, XElem v)
        {
        }

        public void Put(string name, IData v, short msk = 0xff)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                Add('{');
                v.Write(this, msk);
                Add('}');
                level--; // exit
            }
        }

        internal void PutToken(IData v, short msk = 0xff)
        {
            counts[level]++;

            counts[++level] = 0; // enter
            Add('{');
            v.Write(this, msk);

            // append a time stamp
            // Put("$", DateTime.Now);

            Add('}');
            level--; // exit
        }

        public void Put<D>(string name, D[] v, short msk = 0xff) where D : IData
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    Put(null, v[i], msk);
                }
                Add(']');
                level--; // exit
            }
        }

        public void Put<D>(string name, List<D> v, short msk = 0xff) where D : IData
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                Add('[');
                for (int i = 0; i < v.Count; i++)
                {
                    Put(null, v[i], msk);
                }
                Add(']');
                level--; // exit
            }
        }

        public void PutFromSource(ISource v)
        {
            if (counts[level]++ > 0) Add(',');
            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                if (v.IsDataSet)
                {
                    Add('[');
                    bool bgn = false;
                    while (v.Next())
                    {
                        counts[++level] = 0; // enter an data entry
                        if (bgn) Add(',');
                        Add('{');
                        v.Write(this);
                        Add('}');
                        level--;
                        bgn = true;
                    }
                    Add(']');
                }
                else
                {
                    Add('{');
                    v.Write(this);
                    Add('}');
                }
                level--; // exit
            }
        }
    }
}