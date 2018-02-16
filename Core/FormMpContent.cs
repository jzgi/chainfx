using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To generate multipart/form-data binary content, with the part Content-Length extension.
    /// </summary>
    public class FormMpContent : DynamicContent, ISink
    {
        public const string BOUNDARY = "~7^E!3#A&W";

        // deliberately not longer than 40 characters
        const string Mime = "multipart/form-data; boundary=" + BOUNDARY;

        readonly string boundary;

        public FormMpContent(bool pooled, string boundary = "~7^E!3#A&W", int capacity = 1024 * 64) : base(true, capacity)
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

        public void PutOpen()
        {
            throw new NotImplementedException();
        }

        public void PutClose()
        {
            throw new NotImplementedException();
        }

        public void PutStart()
        {
            throw new NotImplementedException();
        }

        public void PutEnd()
        {
            throw new NotImplementedException();
        }

        public void PutNull(string name)
        {
        }

        public void Put(string name, JNumber v)
        {
            Part(name);
            Add(v.bigint);
            if (v.Pt)
            {
                Add('.');
                Add(v.fract);
            }
        }

        public void Put(string name, ISource v)
        {
        }

        public void Put(string name, bool v)
        {
            Part(name);
            Add(v ? "true" : "false");
        }

        public void Put(string name, short v)
        {
            Part(name);
            Add(v);
        }

        public void Put(string name, int v)
        {
            Part(name);
            Add(v);
        }

        public void Put(string name, long v)
        {
            Part(name);
            Add(v);
        }

        public void Put(string name, double v)
        {
            Part(name);
            Add(v);
        }

        public void Put(string name, decimal v)
        {
            Part(name);
            Add(v);
        }

        public void Put(string name, DateTime v)
        {
            Part(name);
            Add(v);
        }

        public void Put(string name, string v)
        {
            Part(name);
            Add(v ?? "null");
        }

        public virtual void Put(string name, ArraySegment<byte> v)
        {
        }

        public void Put(string name, short[] v)
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
        }

        public void Put(string name, int[] v)
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
        }

        public void Put(string name, long[] v)
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
        }

        public void Put(string name, string[] v)
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
                    Add(str ?? "null");
                }
                Add(']');
            }
        }

        public void Put(string name, Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, IData v, byte proj = 0x0f)
        {
            Part(name);
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('{');
                v.Write(this, proj);
                Add('}');
            }
        }

        public void Put<D>(string name, D[] v, byte proj = 0x0f) where D : IData
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
        }
    }
}