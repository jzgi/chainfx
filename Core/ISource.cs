using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A source for data persistence.
    /// </summary>
    public interface ISource
    {

        bool Got(string name, out bool v, bool def = false);

        bool Got(string name, out short v, short def = 0);

        bool Got(string name, out int v, int def = 0);

        bool Got(string name, out long v, long def = -1);

        bool Got(string name, out decimal v, decimal def = 0);

        bool Got(string name, out DateTime v, DateTime def = default(DateTime));

        bool Got(string name, out string v, string def = null);

        bool Got<T>(string name, out T v, T def = default(T)) where T : IPersist, new();

        bool Got<T>(string name, out List<T> v, List<T> def = null) where T : IPersist, new();

        bool Got(string name, out byte[] v, byte[] def = null);

        bool Got(string name, out Obj v, Obj def = null);

        bool Got(string name, out Arr v, Arr def = null);

    }

}