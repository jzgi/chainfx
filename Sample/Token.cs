using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    public class Token : IToken
    {
        internal string id;

        internal string name;

        internal string credential;

        internal bool fame;

        internal bool brand;

        internal bool admin;

        internal DateTime date;

        public string Key => id;

        public string[] Roles => null;

        public string Name => name;

        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(name), ref name);
            sc.Got(nameof(credential), ref credential);
            sc.Got(nameof(fame), ref fame);
            sc.Got(nameof(brand), ref brand);
            sc.Got(nameof(admin), ref admin);
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(name), name);
            sk.Put(nameof(credential), credential);
            sk.Put(nameof(fame), fame);
            sk.Put(nameof(brand), brand);
            sk.Put(nameof(admin), admin);
        }

        public static string Encrypt(string orig)
        {
            return null;
        }

        public static string Decrypt(string src)
        {
            return null;
        }
    }
}