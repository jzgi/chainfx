using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A source for data persistence.
    /// </summary>
    public interface ISource
    {

        bool Got(string name, ref bool v);

        bool Got(string name, ref short v);

        bool Got(string name, ref int v);

        bool Got(string name, ref long v);

        bool Got(string name, ref decimal v);

        bool Got(string name, ref DateTime v);

        bool Got(string name, ref string v);

        bool Got<T>(string name, ref T v) where T : IPersist, new();

        bool Got<T>(string name, ref List<T> v) where T : IPersist, new();

        bool Got(string name, ref byte[] v);

        bool Got(string name, ref Obj v);

        bool Got(string name, ref Arr v);

    }

}