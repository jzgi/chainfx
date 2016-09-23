using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>A user record that is a web access token for all the services. </summary>
    ///
    public class User : IToken, ISerial
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

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(id), ref id);
            r.Read(nameof(name), ref name);
            r.Read(nameof(credential), ref credential);
            r.Read(nameof(fame), ref fame);
            r.Read(nameof(brand), ref brand);
            r.Read(nameof(admin), ref admin);
            r.Read(nameof(date), ref date);
            r.Read(nameof(favposts), ref favposts);
            r.Read(nameof(friends), ref friends);
            r.Read(nameof(favs), ref favs);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(id), id);
            w.Write(nameof(name), name);
            w.Write(nameof(credential), credential);
            w.Write(nameof(fame), fame);
            w.Write(nameof(brand), brand);
            w.Write(nameof(admin), admin);
            w.Write(nameof(date), date);
            w.Write(nameof(favposts), favposts);
            w.Write(nameof(friends), friends);
            w.Write(nameof(favs), favs);
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