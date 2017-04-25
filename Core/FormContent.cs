using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// 
    /// To generate a urlencoded byte or char string.
    /// 
    public class FormContent : DynamicContent, IDataOutput<FormContent>
    {
        public FormContent(bool octal, bool pooled, int capacity = 4092) : base(octal, pooled, capacity)
        {
        }

        public override string Type => "application/x-www-form-urlencoded";

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

        //
        // SINK
        //

        public FormContent PutNull(string name)
        {
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

        public FormContent Put(string name, JNumber v)
        {
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

        public FormContent Put(string name, IDataInput v)
        {
            return this;
        }

        public FormContent PutRaw(string name, string raw)
        {
            return this;
        }

        public void Begin(string label)
        {
        }

        public void End()
        {
        }

        public FormContent Put(string name, bool v, Func<bool, string> Opt = null, string Label = null, bool Required = false)
        {
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

        public FormContent Put(string name, short v, Opt<short> Opt = null, string Label = null, string Help = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
        {
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

        public FormContent Put(string name, int v, Opt<int> Opt = null, string Label = null, string Help = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
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

        public FormContent Put(string name, long v, Opt<long> Opt = null, string Label = null, string Help = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
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

        public FormContent Put(string name, double v, string Label = null, string Help = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
        {
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

        public FormContent Put(string name, decimal v, string Label = null, string Help = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
        {
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

        public FormContent Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
        {
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

        public FormContent Put(string name, string v, Opt<string> Opt = null, string Label = null, string Help = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
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

        public virtual FormContent Put(string name, ArraySegment<byte> v, string Label = null, string Size = null, string Ratio = null, bool Required = false)
        {
            return this; // ignore ir
        }

        public FormContent Put(string name, short[] v, Opt<short> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
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

        public FormContent Put(string name, int[] v, Opt<int> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
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

        public FormContent Put(string name, long[] v, Opt<long> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
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

        public FormContent Put(string name, string[] v, Opt<string> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
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

        public FormContent Put(string name, Dictionary<string, string> v, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            throw new NotImplementedException();
        }

        public FormContent Put(string name, IData v, short proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
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
                Add('{');
                v.WriteData(this, proj);
                Add('}');
            }

            return this;
        }

        public FormContent Put<D>(string name, D[] v, short proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool required = false) where D : IData
        {
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
                    Put(null, v[i], proj);
                }
                Add(']');
            }
            return this;
        }
    }
}