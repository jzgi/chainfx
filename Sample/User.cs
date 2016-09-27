using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>A user record that is a web access token for all the services. </summary>
    ///
    public class User : IToken
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

        public void From(IInput r)
        {
            r.Get(nameof(id), ref id);
            r.Get(nameof(name), ref name);
            r.Get(nameof(credential), ref credential);
            r.Get(nameof(fame), ref fame);
            r.Get(nameof(brand), ref brand);
            r.Get(nameof(admin), ref admin);
            r.Get(nameof(date), ref date);
            r.Get(nameof(favposts), ref favposts);
            r.Get(nameof(friends), ref friends);
            r.Get(nameof(favs), ref favs);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(id), id);
            w.Put(nameof(name), name);
            w.Put(nameof(credential), credential);
            w.Put(nameof(fame), fame);
            w.Put(nameof(brand), brand);
            w.Put(nameof(admin), admin);
            w.Put(nameof(date), date);
            w.Put(nameof(favposts), favposts);
            w.Put(nameof(friends), friends);
            w.Put(nameof(favs), favs);
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