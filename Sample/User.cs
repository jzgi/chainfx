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

        internal List<Fav> fav;

        public string Key => id;

        public string[] Roles => null;

        public string Name => name;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(id), ref id);
            r.Read(nameof(name), ref name);
            r.Read(nameof(credential), ref credential);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(id), id);
            w.Write(nameof(name), name);
            w.Write(nameof(credential), credential);
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