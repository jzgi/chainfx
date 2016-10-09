using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A source for data inputting.
    /// </summary>
    public interface ISource
    {

        bool Got(string name, ref bool v);

        bool Got(string name, ref short v);

        bool Got(string name, ref int v);

        bool Got(string name, ref long v);

        bool Got(string name, ref decimal v);

        bool Got(string name, ref Number v);

        bool Got(string name, ref DateTime v);

        bool Got(string name, ref char[] v);

        bool Got(string name, ref string v);

        bool Got(string name, ref byte[] v);

        bool Got<T>(string name, ref T v, int x = -1) where T : IPersist, new();

        bool Got(string name, ref JObj v);

        bool Got(string name, ref JArr v);

        bool Got(string name, ref short[] v);

        bool Got(string name, ref int[] v);

        bool Got(string name, ref long[] v);

        bool Got(string name, ref string[] v);

        bool Got<T>(string name, ref T[] v, int x = -1) where T : IPersist, new();

    }

}