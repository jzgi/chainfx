using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data outputing destination.
    /// </summary>
    public interface IResultSet : ISource
    {
        T Get<T>() where T : IPersist, new();

        List<T> GetList<T>() where T : IPersist, new();

        bool Got(out bool v, bool def = false);

        bool Got(out short v, short def = 0);

        bool Got(out int v, int def = 0);

        bool Got(out long v, long def = 0);

        bool Got(out decimal v, decimal def = 0);

        bool Got(out DateTime v, DateTime def = default(DateTime));

        bool Got(out string v, string def = null);

        bool Got<T>(out T v, T def = default(T)) where T : IPersist, new();

        bool Got<T>(out List<T> v, List<T> def = null) where T : IPersist, new();

        bool Got(out byte[] v, byte[] def = null);

        bool Got(out Obj v, Obj def = null);

        bool Got(out Arr v, Arr def = null);

    }
}