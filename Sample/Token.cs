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


        public static string Encrypt(int tok, string credential)
        {
            int clen;
            if (credential == null || (clen = credential.Length) != 16) return null;

            char[] buf = new char[8 + clen];
            int p = 0;
            // append id in hex format
            for (int i = 7; i >= 0; i--)
            {
                buf[p++] = HEX[(tok >> (i * 4)) & 0x0f];
            }
            // append crendential 
            for (int i = 0; i < clen; i++) { buf[p++] = credential[i]; }
            return new string(buf);
        }

        public static bool Decrypt(string tok)
        {
        
            return true;
        }

    }
}