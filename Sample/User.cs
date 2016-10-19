using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    public class User : IPersist
    {
        public const uint X_MGT = 0x01;

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

        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(name), ref name);
            s.Got(nameof(credential), ref credential);
            s.Got(nameof(fame), ref fame);
            s.Got(nameof(brand), ref brand);
            s.Got(nameof(admin), ref admin);
            if ((x & X_MGT) == x)
            {
                s.Got(nameof(date), ref date);
            }
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(credential), credential);
            s.Put(nameof(fame), fame);
            s.Put(nameof(brand), brand);
            s.Put(nameof(admin), admin);
            if ((x & X_MGT) == x)
            {
                s.Put(nameof(date), date);
            }
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