using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class User : IToken, IDump, IZone
    {
        string id;

        string name;

        string mobile;

        List<Perm> perms;

        public void From(IInput i)
        {
            i.Got(nameof(id), out id);
            i.Got(nameof(name), out name);
            i.Got(nameof(mobile), out mobile);
            i.Got(nameof(perms), out perms);
        }

        public void To(IOutput o)
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(mobile), mobile);
            o.Put(nameof(perms), perms);
        }


        public static string Encrypt(string orig)
        {
            return null;
        }

        public static string Decrypt(string src)
        {
            return null;
        }

        public bool Can(string zone, int role)
        {
            return false;
        }

        public long ModifiedOn { get; set; }
        public string Key { get; }

        struct Perm : IDump
        {
            string org;

            int role;

            public void From(IInput i)
            {
                i.Got(nameof(org), out org);
                i.Got(nameof(role), out role);
            }

            public void To(IOutput o)
            {
                o.Put(nameof(org), org);
                o.Put(nameof(role), role);
            }
        }
    }
}