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

        public JsonContent(bool octet, int capacity = 8 * 1024) : base(octet, capacity)
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

        public void OBJ(Action<JsonContent> a)
        {
            if (counts[level]++ > 0) Add(',');

            counts[++level] = 0; // enter
            Add('{');

            a?.Invoke(this);

            Add('}');
            level--; // exit
        }

        //
        // SINK
        //

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

        public JsonContent Put(string name, JNumber value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add(value.bigint);
            if (value.Pt)
            {
                Add('.');
                Add(value.fract);
            }
            return this;
        }

        public JsonContent Put(string name, IDataInput value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            if (value == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter

                if (value.DataSet)
                {
                    Add('[');
                    bool bgn = false;
                    while (value.Next())
                    {
                        counts[++level] = 0; // enter an data entry

                        if (bgn) Add(',');

                        Add('{');
                        value.Write(this);
                        Add('}');

                        level--;
                        bgn = true;
                    }
                    Add(']');
                }
                else
                {
                    Add('{');

                    value.Write(this);

                    Add('}');
                }

                level--; // exit
            }
            return this;
        }

        public JsonContent Put(string name, bool value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(value ? "true" : "false");
            return this;
        }

        public JsonContent Put(string name, short value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(value);
            return this;
        }

        public JsonContent Put(string name, int value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(value);
            return this;
        }

        public JsonContent Put(string name, long value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(value);
            return this;
        }

        public JsonContent Put(string name, double value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(value);
            return this;
        }

        public JsonContent Put(string name, decimal value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            Add(value);
            return this;
        }

        public JsonContent Put(string name, DateTime value)
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
            Add(value);
            Add('"');
            return this;
        }

        public JsonContent Put(string name, string value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('"');
                AddEsc(value);
                Add('"');
            }
            return this;
        }

        public virtual JsonContent Put(string name, ArraySegment<byte> value)
        {
            return this; // ignore ir
        }

        public JsonContent Put(string name, short[] value)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < value.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(value[i]);
                }
                Add(']');
            }
            return this;
        }

        public JsonContent Put(string name, int[] value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < value.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(value[i]);
                }
                Add(']');
            }
            return this;
        }

        public JsonContent Put(string name, long[] value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < value.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(value[i]);
                }
                Add(']');
            }
            return this;
        }

        public JsonContent Put(string name, string[] value)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < value.Length; i++)
                {
                    if (i > 0) Add(',');
                    string str = value[i];
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


        public JsonContent Put(string name, Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }

        public JsonContent Put(string name, IData value, int proj = 0x00ff)
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (value == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                Add('{');

                // put shard property if any
                string shard = (value as IShardable)?.Shard;
                if (shard != null)
                {
                    Put("#", shard);
                }

                value.Write(this, proj);
                Add('}');
                level--; // exit
            }
            return this;
        }

        public JsonContent Put<D>(string name, D[] value, int proj = 0x00ff) where D : IData
        {
            if (counts[level]++ > 0) Add(',');
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }
            if (value == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                Add('[');
                for (int i = 0; i < value.Length; i++)
                {
                    Put(null, value[i], proj);
                }
                Add(']');
                level--; // exit
            }
            return this;
        }
    }
}