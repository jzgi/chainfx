using System;

namespace ChainFx
{
    /// <summary>
    /// To generate a urlencoded byte or char string.
    /// </summary>
    public class FormBuilder : ContentBuilder, ISink
    {
        // hexidecimal characters
        protected static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        int ordinal = -1;

        public FormBuilder(bool bytely, int capacity) : base(bytely, capacity)
        {
        }

        public override string CType { get; set; } = "application/x-www-form-urlencoded";

        void AddEsc(string v)
        {
            if (v == null) return;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9') // alphabetic and decimal digits
                {
                    Add(c);
                }
                else if (c == '.' || c == '-' || c == '*' || c == '_')
                {
                    Add(c);
                }
                else if (c == ' ')
                {
                    Add('+');
                }
                else
                {
                    AddPercent(c);
                }
            }
        }

        void AddPercent(char c)
        {
            if (c < 0x80)
            {
                // have at most seven bits
                AddEscByte((byte) c);
            }
            else if (c < 0x800)
            {
                // 2 char, 11 bits
                AddEscByte((byte) (0xc0 | (c >> 6)));
                AddEscByte((byte) (0x80 | (c & 0x3f)));
            }
            else
            {
                // 3 char, 16 bits
                AddEscByte((byte) (0xe0 | ((c >> 12))));
                AddEscByte((byte) (0x80 | ((c >> 6) & 0x3f)));
                AddEscByte((byte) (0x80 | (c & 0x3f)));
            }
        }

        void AddEscByte(byte b)
        {
            Add('%');
            Add(HEX[(b >> 4) & 0x0f]);
            Add(HEX[b & 0x0f]);
        }

        public void AddNonAscii(string v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                var c = v[i];
                if (c < 128)
                {
                    Add(c);
                }
                else
                {
                    AddPercent(c);
                }
            }
        }

        //
        // SINK
        //

        public void PutNull(string name)
        {
        }

        public void Put(string name, JNumber v)
        {
        }

        public void Put(string name, ISource v)
        {
        }

        public void Put(string name, bool v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v ? "true" : "false");
        }

        public void Put(string name, char v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
        }

        public void Put(string name, short v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
        }

        public void Put(string name, int v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
        }

        public void Put(string name, long v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
        }

        public void Put(string name, double v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
        }

        public void Put(string name, decimal v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
        }

        public void Put(string name, DateTime v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
        }

        public void Put(string name, string v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            AddEsc(v);
        }

        public void Put(string name, ArraySegment<byte> v)
        {
        }

        public void Put(string name, byte[] v)
        {
        }

        public void Put(string name, short[] v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }
        }

        public void Put(string name, int[] v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }
        }

        public void Put(string name, long[] v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }
        }

        public void Put(string name, string[] v)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }
        }

        public void Put(string name, JObj v)
        {
        }

        public void Put(string name, JArr v)
        {
        }

        public void Put(string name, XElem v)
        {
        }

        public void Put(string name, IData v, short proj = 0xff)
        {
        }

        public void Put<D>(string name, D[] v, short proj = 0xff) where D : IData
        {
        }

        public void PutFromSource(ISource s)
        {
        }
    }
}