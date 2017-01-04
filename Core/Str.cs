namespace Greatbone.Core
{
    ///
    /// A reusable string builder that supports UTF-8 decoding.
    ///
    public class Str : DynamicContent
    {
        int sum; // combination of bytes

        int rest; // number of rest octets

        public Str(int capacity) : base(false, false, capacity)
        {
            sum = 0;
            rest = 0;
        }

        public override string MimeType => null;

        // utf-8 decoding 
        public void Accept(int b)
        {
            if (rest == 0)
            {
                if (b < 0x80)
                {
                    Add((char) b); // single byte
                }
                else if (b >= 0xc0 && b < 0xe0)
                {
                    sum = (b & 0x1f) << 6;
                    rest = 1;
                }
                else if (b >= 0xe0 && b < 0xf0)
                {
                    sum = (b & 0x0f) << 12;
                    rest = 2;
                }
            }
            else if (rest == 1)
            {
                sum |= (b & 0x3f);
                rest--;
                Add((char) sum);
            }
            else if (rest == 2)
            {
                sum |= (b & 0x3f) << 6;
                rest--;
            }
        }

        public void Clear()
        {
            count = 0;
            sum = 0;
            rest = 0;
        }

        public bool Match(string v)
        {
            return false;
        }

        public int ToInt()
        {
            return 0;
        }

        public override string ToString()
        {
            return new string(charbuf, 0, count);
        }
    }
}