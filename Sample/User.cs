using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
  public class User : IToken, IZone
  {
    // id
    string login;

    string name;

    string mobile;

    List<Perm> perms;


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

    struct Perm
    {
      string org;

      int role;

    }
  }
}