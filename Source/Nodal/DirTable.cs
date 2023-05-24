using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChainFx.Nodal;

/// <summary>
/// A data table based on a certain directory.
/// </summary>
public class DirTable : IKeyable<string>
{
    JObj Find(string pk)
    {
        return null;
    }

    E Find<E>(string pk)
    {
        return default;
    }

    IList<JObj> FindAll(string pk, Predicate<string> nameflt)
    {
        return null;
    }

    E[] FindAll<E>(string pk, Predicate<string> nameflt)
    {
        return null;
    }

    public async Task<bool> TryUpdateAsync(string pk, JObj v)
    {
        return false;
    }

    public async Task<JObj> TryRemoveAsync(string pk)
    {
        return null;
    }

    public string Key { get; }
}