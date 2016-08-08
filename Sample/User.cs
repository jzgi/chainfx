using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
  public class User : IToken, IData, IZone
  {
    // id
    string login;

    string name;

    string mobile;

    List<Perm> perms;

    public void From(IDataInput i)
    {
      i.Got(nameof(login), ref login);
      i.Got(nameof(name), ref name);
      i.Got(nameof(mobile), ref mobile);
      i.Got(nameof(perms), ref perms);
    }

    public void To(IDataOutput o)
    {
      o.Put(nameof(login), login);
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

    struct Perm : IData
    {
      string org;

      int role;

      public void From(IDataInput i)
      {
        i.Got(nameof(org), ref org);
        i.Got(nameof(role), ref role);
      }

      public void To(IDataOutput o)
      {
        o.Put(nameof(org), org);
        o.Put(nameof(role), role);
      }
    }
  }
}