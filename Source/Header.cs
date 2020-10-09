namespace Skyiah
{
    /// <summary>
    /// To analyze HTTP header.
    /// </summary>
    public class Header : Text
    {
        // start index of the value field
        int vstart;

        public Header(int capacity = 256) : base(capacity)
        {
        }

        public override void Clear()
        {
            base.Clear();
            vstart = 0;
        }

        /// Check if it is valid with the given header name.
        ///
        public bool Check(string hdrname)
        {
            int len = hdrname.Length;

            if (count < len + 2) return false; // name + :SP

            for (int i = 0; i < len; i++)
            {
                if (charbuf[i] != hdrname[i])
                {
                    return false;
                }
            }
            if (charbuf[len] != ':') return false;

            // markdown the value start
            int p = len + 1;
            if (charbuf[p] == ' ') p++;
            vstart = p;

            return true;
        }

        public string GetVvalue() => new string(charbuf, vstart, count - vstart);

        static bool IsEdge(char c)
        {
            return c == ' ' || c == ',' || c == ';';
        }

        public string SeekParam(string param)
        {
            int len = param.Length;

            int p = vstart;
            for (;;)
            {
                // seek for an eq
                for (;;)
                {
                    if (p >= count) return null;
                    if (charbuf[p] == '=') break;
                    p++;
                }

                int eq = p;
                int first = eq - len; // first char
                bool match = true;
                for (int i = 0; i < len; i++)
                {
                    if (charbuf[first + i] != param[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (!match || !IsEdge(charbuf[first - 1]))
                {
                    p = eq + 2; // adjust position
                    continue;
                }

                // get param-value after eq
                int v0 = ++p;
                bool quot = charbuf[p] == '"';
                if (quot)
                {
                    for (;;)
                    {
                        if (charbuf[++p] == '"')
                        {
                            return new string(charbuf, v0 + 1, p - v0 - 1);
                        }
                    }
                }
                else // parse till sep for non-quoted
                {
                }
            }
        }
    }
}