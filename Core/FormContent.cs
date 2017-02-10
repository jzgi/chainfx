using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    /// 
    /// To generate a urlencoded byte or char string.
    /// 
    public class FormContent : DynamicContent, IDataOutput<FormContent>
    {
        public FormContent(bool sendable = true, bool pooled = true, int capacity = 4092) : base(sendable, pooled, capacity)
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


        public FormContent PutEnter(bool multi)
        {
            throw new NotImplementedException();
        }

        public FormContent PutExit(bool multi)
        {
            throw new NotImplementedException();
        }

        public FormContent PutRaw(string name, string raw)
        {
            throw new NotImplementedException();
        }

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

        public FormContent Put(string name, bool v, string Label = null, bool Required = false)
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

        public FormContent Put(string name, short v, string Label = null, bool Pick = false, string Placeholder = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
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

        public FormContent Put(string name, int v, string Label = null, bool Pick = false, string Placeholder = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
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

        public FormContent Put(string name, long v, string Label = null, bool Pick = false, string Placeholder = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
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

        public FormContent Put(string name, double v, string Label = null, string Placeholder = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
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

        public FormContent Put(string name, decimal v, string Label = null, string Placeholder = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
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

        public FormContent Put(string name, NpgsqlPoint v)
        {
            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add('"');
            Add(v.X);
            Add(':');
            Add(v.Y);
            Add('"');

            return this;
        }

        public FormContent Put(string name, char[] v)
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
                Add(v);
                Add('"');
            }

            return this;
        }

        public FormContent Put(string name, string v, string Label = null, bool Pick = false, string Placeholder = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
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

        public virtual FormContent Put(string name, byte[] v)
        {
            return this; // ignore ir
        }

        public virtual FormContent Put(string name, ArraySegment<byte> v)
        {
            return this; // ignore ir
        }

        public FormContent Put(string name, IData v, ushort proj = 0)
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

        public FormContent Put(string name, JObj v)
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
                v.WriteData(this);
                Add('}');
            }

            return this;
        }

        public FormContent Put(string name, JArr v)
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
                v.WriteData(this);
                Add(']');
            }
            return this;
        }

        public FormContent Put(string name, short[] v)
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

        public FormContent Put(string name, int[] v)
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

        public FormContent Put(string name, long[] v)
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

        public FormContent Put(string name, string[] v)
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


        public FormContent Put<D>(string name, D[] v, ushort proj = 0) where D : IData
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

        public FormContent Put<D>(string name, List<D> v, ushort proj = 0) where D : IData
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
                for (int i = 0; i < v.Count; i++)
                {
                    Put(null, v[i], proj);
                }
                Add(']');
            }
            return this;
        }

        public FormContent Put(string name, IDataInput v)
        {
            throw new NotImplementedException();
        }

        public FormContent Put(string name, Dictionary<string, string> v)
        {
            throw new NotImplementedException();
        }
    }
}