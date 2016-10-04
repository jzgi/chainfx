using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data outputing destination.
    /// </summary>
    public interface ISink<R> where R : ISink<R>
    {

        R PutNull(string name);

        R Put(string name, bool v);

        R Put(string name, short v);

        R Put(string name, int v);

        R Put(string name, long v);

        R Put(string name, decimal v);

        R Put(string name, DateTime v);

        R Put(string name, string v);

        R Put<T>(string name, T v, int x = -1) where T : IPersist;

        R Put<T>(string name, List<T> v, int x = -1) where T : IPersist;

        R Put(string name, byte[] v);

        R Put(string name, Obj v);

        R Put(string name, Arr v);

    }

}