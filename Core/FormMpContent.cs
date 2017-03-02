using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    /// 
    /// To generate multipart/form-data binary content, with the part Content-Length extension.
    /// 
    public class FormMpContent : DynamicContent, IDataOutput<FormMpContent>
    {
        public const string BOUNDARY = "~7^E!3#A&W";

        // deliberately not longer than 40 characters
        const string Mime = "multipart/form-data; boundary=" + BOUNDARY;

        readonly string boundary;

        public FormMpContent(bool pooled, string boundary = "~7^E!3#A&W", int capacity = 1024 * 256) : base(true, pooled, capacity)
        {
            this.boundary = boundary;
        }

        public override string Type => Mime;

        //
        // SINK
        //

        void Part(string name)
        {
            Add(BOUNDARY);
            Add("Content-Disposition: form-data; name=\"");
            Add(name);
            Add("\"\r\n\r\n");
        }

        public FormMpContent PutNull(string name)
        {
            return this;
        }

        public FormMpContent PutRaw(string name, string raw)
        {
            throw new NotImplementedException();
        }

        public FormMpContent Put(string name, bool v, Func<bool, string> Options = null, string Label = null, bool Required = false)
        {
            Part(name);
            Add(v ? "true" : "false");
            return this;
        }

        public FormMpContent Put(string name, short v, IDictionary<short, string> Options = null, string Label = null, string Placeholder = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, int v, IDictionary<int, string> Options = null, string Label = null, string Placeholder = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, long v, IDictionary<long, string> Options = null, string Label = null, string Placeholder = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, double v, string Label = null, string Placeholder = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, decimal v, string Label = null, string Placeholder = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, JNumber v)
        {
            Part(name);
            Add(v.bigint);
            if (v.Pt)
            {
                Add('.');
                Add(v.fract);
            }
            return this;
        }

        public FormMpContent Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, NpgsqlPoint v)
        {
            Part(name);
            Add(v.X);
            Add(',');
            Add(v.Y);
            return this;
        }

        public FormMpContent Put(string name, char[] v)
        {
            Part(name);
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public FormMpContent Put(string name, string v, IDictionary<string, string> Options = null, string Label = null, string Placeholder = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public virtual FormMpContent Put(string name, byte[] v)
        {
            return this; // ignore ir
        }

        public virtual FormMpContent Put(string name, ArraySegment<byte> v)
        {
            return this; // ignore ir
        }

        public FormMpContent Put(string name, JObj v)
        {
            Part(name);
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

        public FormMpContent Put(string name, JArr v)
        {
            Part(name);
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

        public FormMpContent Put(string name, short[] v, IDictionary<short, string> Options = null, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
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

        public FormMpContent Put(string name, int[] v, IDictionary<int, string> Options = null, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
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

        public FormMpContent Put(string name, long[] v, IDictionary<long, string> Options = null, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
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

        public FormMpContent Put(string name, string[] v, IDictionary<string, string> Options = null, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
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
                        Add(str);
                    }
                }
                Add(']');
            }

            return this;
        }

        public FormMpContent Put(string name, Dictionary<string, string> v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            throw new NotImplementedException();
        }

        public FormMpContent Put(string name, IData v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            Part(name);
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

        public FormMpContent Put<D>(string name, D[] v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            Part(name);
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

        public FormMpContent Put<D>(string name, List<D> v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            Part(name);
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

        public void PutEvent(long id, string name, string shard, string arg, DateTime time, IContent content)
        {

        }

        public FormMpContent Put(string name, IDataInput v)
        {
            throw new NotImplementedException();
        }

        public FormMpContent Put(string name, Diction v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            throw new NotImplementedException();
        }

        public FormMpContent Put<D>(string name, Map<D> v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            throw new NotImplementedException();
        }
    }
}