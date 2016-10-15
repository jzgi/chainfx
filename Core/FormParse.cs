
using System;

namespace Greatbone.Core
{

    public struct FormParse
    {
        static readonly JException FormatEx = new JException("form Format");

        readonly byte[] buffer;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public FormParse(ArraySegment<byte> bytes)
        {
            this.buffer = bytes.Array;
            this.count = bytes.Count;
            this.str = new Str();
        }

        public Form Parse()
        {
            return null;
        }

        byte[] ParseBytes(int start)
        {
            return null;
        }

        bool ParseNull(int start)
        {
            int p = start;
            if (buffer[++p] == 'u' && buffer[++p] == 'l' && buffer[++p] == 'l')
            {
                return true;
            }
            return false;
        }

        Number ParseNumber(int start)
        {
            return default(Number);
        }

        bool ParseBool(int start)
        {
            int p = start;
            if (buffer[++p] == 'u' && buffer[++p] == 'l' && buffer[++p] == 'l')
            {
                return true;
            }
            return false;
        }
    }
}