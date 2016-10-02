using System.Text;

namespace Greatbone.Core
{

    public struct FormParse
    {
        static readonly FormatException FormatEx = new FormatException("JSON Format");

        readonly byte[] buffer;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public FormParse(byte[] buffer) : this(buffer, buffer.Length) { }

        public FormParse(byte[] buffer, int count)
        {
            this.buffer = buffer;
            this.count = count;
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