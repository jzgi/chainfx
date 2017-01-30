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
        public const string Boundary = "~7^E!3#A&W";

        // deliberately not longer than 40 characters
        const string Mime = "multipart/form-data; boundary=" + Boundary;

        public FormMpContent(bool pooled, int capacity = 1024 * 256) : base(true, pooled, capacity)
        {
        }

        public override string Type => Mime;

        //
        // SINK
        //

        void Part(string name)
        {
            Add(Boundary);
            Add("Content-Disposition: form-data; name=\"");
            Add(name);
            Add("\"\r\n\r\n");
        }

        public FormMpContent PutEnter(bool multi)
        {
            throw new NotImplementedException();
        }

        public FormMpContent PutExit(bool multi)
        {
            throw new NotImplementedException();
        }

        public FormMpContent PutNull(string name)
        {
            return this;
        }

        public FormMpContent PutRaw(string name, string raw)
        {
            throw new NotImplementedException();
        }

        public FormMpContent Put(string name, bool v)
        {
            Part(name);
            Add(v ? "true" : "false");
            return this;
        }

        public FormMpContent Put(string name, short v)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, int v)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, long v)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, double v)
        {
            Part(name);
            Add(v);
            return this;
        }

        public FormMpContent Put(string name, decimal v)
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

        public FormMpContent Put(string name, DateTime v)
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

        public FormMpContent Put(string name, string v, bool? anylen = null)
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

        public FormMpContent Put(string name, IData v, byte flags = 0)
        {
            Part(name);
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('{');
                v.WriteData(this, flags);
                Add('}');
            }
            return this;
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

        public FormMpContent Put(string name, short[] v)
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

        public FormMpContent Put(string name, int[] v)
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

        public FormMpContent Put(string name, long[] v)
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

        public FormMpContent Put(string name, string[] v)
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

        public FormMpContent Put<D>(string name, D[] v, byte flags = 0) where D : IData
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
                    Put(null, v[i], flags);
                }
                Add(']');
            }
            return this;
        }

        public FormMpContent Put<D>(string name, List<D> v, byte flags = 0) where D : IData
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
                    Put(null, v[i], flags);
                }
                Add(']');
            }
            return this;
        }

        public void PutEvent(long id, string name, DateTime time, string mtype, ArraySegment<byte> body)
        {

        }

        public FormMpContent Put(string name, IModel v)
        {
            throw new NotImplementedException();
        }
    }
}