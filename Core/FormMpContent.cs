using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// To generate multipart/form-data binary content, with the part Content-Length extension.
    /// </summary>
    public class FormMpContent : DynamicContent, IDataOutput<FormMpContent>
    {
        public const string BOUNDARY = "~7^E!3#A&W";

        // deliberately not longer than 40 characters
        const string Mime = "multipart/form-data; boundary=" + BOUNDARY;

        readonly string boundary;

        public FormMpContent(bool pooled, string boundary = "~7^E!3#A&W", int capacity = 1024 * 256) : base(true, capacity)
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

        public FormMpContent Put(string name, JNumber value)
        {
            Part(name);
            Add(value.bigint);
            if (value.Pt)
            {
                Add('.');
                Add(value.fract);
            }
            return this;
        }

        public FormMpContent Put(string name, IDataInput value)
        {
            return this;
        }

        public FormMpContent PutRaw(string name, string raw)
        {
            return this;
        }

        public void Group(string label)
        {
        }

        public void UnGroup()
        {
        }

        public FormMpContent Put(string name, bool value, string Label = null, Func<bool, string> Opt = null)
        {
            Part(name);
            Add(value ? "true" : "false");
            return this;
        }

        public FormMpContent Put(string name, short value, string Label = null, IOptable<short> opt = null)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, int value, string Label = null, IOptable<int> Opt = null)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, long value, string Label = null, IOptable<long> opt = null)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, double value, string label = null)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, decimal value, string label = null, char format = '\0')
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, DateTime value, string label = null)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, string value, string Label = null, IOptable<string> Opt = null)
        {
            Part(name);
            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add(value);
            }
            return this;
        }

        public virtual FormMpContent Put(string name, ArraySegment<byte> value, string label = null)
        {
            return this; // ignore ir
        }

        public FormMpContent Put(string name, short[] value, string Label = null, IOptable<short> opt = null)
        {
            Part(name);
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

        public FormMpContent Put(string name, int[] value, string Label = null, IOptable<int> Opt = null)
        {
            Part(name);
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

        public FormMpContent Put(string name, long[] value, string Label = null, IOptable<long> Opt = null)
        {
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

        public FormMpContent Put(string name, string[] value, string Label = null, IOptable<string> Opt = null)
        {
            Part(name);
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
                        Add(str);
                    }
                }
                Add(']');
            }

            return this;
        }

        public FormMpContent Put(string name, Dictionary<string, string> value, string label = null)
        {
            throw new NotImplementedException();
        }

        public FormMpContent Put(string name, IData value, int proj = 0x00ff, string Label = null)
        {
            Part(name);
            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('{');
                value.Write(this, proj);
                Add('}');
            }
            return this;
        }

        public FormMpContent Put<D>(string name, D[] value, int proj = 0x00ff, string Label = null) where D : IData
        {
            Part(name);
            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('[');
                for (int i = 0; i < value.Length; i++)
                {
                    Put(null, value[i], proj);
                }
                Add(']');
            }
            return this;
        }

        public void PutEvent(long id, string name, string shard, string arg, DateTime time, IContent content)
        {
        }
    }
}