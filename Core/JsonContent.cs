using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// To generate a UTF-8 encoded JSON document. An extension of putting byte array is supported.
    ///
    public class JsonContent : DynamicContent, IDataOutput<JsonContent>
    {
        // starting positions of each level
        readonly int[] counts;

        // current level
        int level;

        public JsonContent() : this(true, true)
        {
        }

        public JsonContent(bool octal, bool pooled, int capacity = 8 * 1024) : base(octal, pooled, capacity)
        {
            counts = new int[8];
            level = 0;
        }

        public override string Type => "application/json; charset=utf-8";

        void AddEsc(string v)
        {
            if (v == null) return;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
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


        public void ARR(Action a)
        {
            if (counts[level]++ > 0) Add(',');

            counts[++level] = 0; // enter
            Add('[');

            a?.Invoke();

            Add(']');
            level--; // exit
        }

        public void OBJ(Action a)
        {
            if (counts[level]++ > 0) Add(',');

            counts[++level] = 0; // enter
            Add('{');

            a?.Invoke();

            Add('}');
            level--; // exit
        }

        //
        // SINK
        //

        public JsonContent PutRaw(string name, string raw)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(raw ?? "null");
            return this;
        }

        public void Group(string label)
        {
        }

        public void UnGroup()
        {
        }

        public JsonContent PutNull(string name)
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
            return this;
        }

        public JsonContent Put(string name, JNumber v)
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
            return this;
        }

        public JsonContent Put(string name, IDataInput v)
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

                if (v.DataSet)
                {
                    Add('[');
                    bool bgn = false;
                    while (v.Next())
                    {
                        counts[++level] = 0; // enter an data entry

                        if (bgn) Add(',');

                        Add('{');
                        v.WriteData(this);
                        Add('}');

                        level--;
                        bgn = true;
                    }
                    Add(']');
                }
                else
                {
                    Add('{');

                    v.WriteData(this);

                    Add('}');
                }

                level--; // exit
            }
            return this;
        }

        public JsonContent Put(string name, bool v, Func<bool, string> Opt = null, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, short v, Opt<short> Opt = null, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, int v, Opt<int> Opt = null, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, long v, Opt<long> Opt = null, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, double v, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, decimal v, string Label = null, char format = '\0')
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
            return this;
        }

        public JsonContent Put(string name, DateTime v, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, string v, Opt<string> Opt = null, string Label = null)
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
            return this;
        }

        public virtual JsonContent Put(string name, ArraySegment<byte> v, string Label = null)
        {
            return this; // ignore ir
        }

        public JsonContent Put(string name, short[] v, Opt<short> Opt = null, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, int[] v, Opt<int> Opt = null, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, long[] v, Opt<long> Opt = null, string Label = null)
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
            return this;
        }

        public JsonContent Put(string name, string[] v, Opt<string> Opt = null, string Label = null)
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
            return this;
        }


        public JsonContent Put(string name, Dictionary<string, string> v, string Label = null)
        {
            throw new NotImplementedException();
        }

        public JsonContent Put(string name, IData v, short proj = 0, string Label = null)
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

                // put shard property if any
                string shard = (v as IShardable)?.Shard;
                if (shard != null)
                {
                    Put("#", shard);
                }

                v.WriteData(this, proj);
                Add('}');
                level--; // exit
            }
            return this;
        }

        public JsonContent Put<D>(string name, D[] v, short proj = 0, string Label = null) where D : IData
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
                    Put(null, v[i], proj);
                }
                Add(']');
                level--; // exit
            }
            return this;
        }
    }
}