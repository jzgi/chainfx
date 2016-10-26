using System;
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


        public static void Encrypt(DynamicContent cont, int mask, int order)
        {
            int[] masks = { mask >> 24 & 0xff, mask >> 16 & 0xff, mask >> 8 & 0xff, mask & 0xff };
            byte[] buffer = cont.Buffer;
            int len = cont.Length;
            byte[] buf = new byte[len * 2]; // the target bytebuf
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // masking
                int b = buffer[i] ^ masks[i % 4];

                //transform
                buf[p++] = (byte)HEX[(b >> 8) & 0x0f];
                buf[p++] = (byte)HEX[(b) & 0x0f];

                // reordering

            }

            // replace
            cont.Replace(buf, p);
        }


        public static string Decrypt(string tokstr, int mask, int order)
        {
            int[] masks = { mask >> 24 & 0xff, mask >> 16 & 0xff, mask >> 8 & 0xff, mask & 0xff };
            int len = tokstr.Length / 2;
            char[] buf = new char[len]; // the target charbuf
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // reordering

                // transform
                int b = V(tokstr[p++]) << 8 | V(tokstr[p++]);

                // masking
                buf[i] = (char)(b ^ masks[i % 4]);
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