using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChainFx.Nodal;

public class FsTable : IKeyable<string>
{
    JObj Find(string pk)
    {
        return null;
    }

    IList<JObj> FindAll(string pk, Predicate<string> namefltr)
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