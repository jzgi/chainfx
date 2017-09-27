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

        public FormMpContent Put(string name, bool value)
        {
            Part(name);
            Add(value ? "true" : "false");
            return this;
        }

        public FormMpContent Put(string name, short value)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, int value)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, long value)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, double value)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, decimal value)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, DateTime value)
        {
            Part(name);
            Add(value);
            return this;
        }

        public FormMpContent Put(string name, string value)
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

        public virtual FormMpContent Put(string name, ArraySegment<byte> value)
        {
            return this; // ignore ir
        }

        public FormMpContent Put(string name, short[] value)
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

        public FormMpContent Put(string name, int[] value)
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

        public FormMpContent Put(string name, long[] value)
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

        public FormMpContent Put(string name, string[] value)
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

        public FormMpContent Put(string name, Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }

        public FormMpContent Put(string name, IData value, int proj = 0x00ff)
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

        public FormMpContent Put<D>(string name, D[] value, int proj = 0x00ff) where D : IData
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