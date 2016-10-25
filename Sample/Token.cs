using Greatbone.Core;

namespace Greatbone.Sample
{

    /// <summary>
    /// Used by all services.
    /// </summary>
    public class Token : IToken, IPersist
    {
        internal string id;
        internal string name;
        internal string credential;
        internal bool fame;
        internal bool brand;
        internal bool admin;

        public string Key => id;

        public string Name => name;

        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(name), ref name);
            s.Got(nameof(credential), ref credential);
            s.Got(nameof(fame), ref fame);
            s.Got(nameof(brand), ref brand);
            s.Got(nameof(admin), ref admin);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            if (x.ExtraOn())
            {
                s.Put(nameof(credential), credential);
            }
            s.Put(nameof(fame), fame);
            s.Put(nameof(brand), brand);
            s.Put(nameof(admin), admin);
        }


        // hexidecimal numbers
        static readonly char[] HEX = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };


        public static string Encrypt(string json)
        {
            int len = json.Length;
            char[] buf = new char[len * 2];
            for (int i = 0, j = 0; i < len; i++)
            {
                uint v = json[i];
                buf[j++] = HEX[(v >> 8) & 0x0f];
                buf[j++] = HEX[(v) & 0x0f];
            }
            return new string(buf);
        }

        public static string Decrypt(string tok)
        {
            int len = tok.Length / 2;
            char[] buf = new char[len];
            for (int i = 0, j = 0; i < len; i++)
            {
                buf[i] = (char)(V(tok[j++]) << 8 | V(tok[j++]));
            }
            return new string(buf);
        }

        static int V(char h)
        {
            int v = h - '0';
            if (v <= 9)
            {
                return v;
            }
            else
            {
                v = h - 'a';
                if (v <= 6) return v + 10;
            }
            return 0;
        }
    }
}