using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>A user record that is a web access token for all the services. </summary>
    ///
    public class User : IToken, IData
    {
        internal string id;

        internal string name;

        internal string credential;

        internal bool fame;

        internal bool brand;

        internal bool admin;

        internal DateTime date;

        internal List<FavPost> favposts;

        internal List<Fav> friends;

        internal List<Fav> favs;

        public string Key => id;

        public string[] Roles => null;

        public string Name => name;

        public void In(IDataIn i)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            i.Get(nameof(credential), ref credential);
            i.Get(nameof(fame), ref fame);
            i.Get(nameof(brand), ref brand);
            i.Get(nameof(admin), ref admin);
            i.Get(nameof(date), ref date);
            i.Get(nameof(favposts), ref favposts);
            i.Get(nameof(friends), ref friends);
            i.Get(nameof(favs), ref favs);
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(credential), credential);
            o.Put(nameof(fame), fame);
            o.Put(nameof(brand), brand);
            o.Put(nameof(admin), admin);
            o.Put(nameof(date), date);
            o.Put(nameof(favposts), favposts);
            o.Put(nameof(friends), friends);
            o.Put(nameof(favs), favs);
        }

        public static string Encrypt(string orig)
        {
            return null;
        }

        public static string Decrypt(string src)
        {
            return null;
        }

        public struct Fav
        {
            internal char[] id;
        }

        public struct FavPost
        {
            internal int id;
        }
    }
}