using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents A source for object persistence.
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

        bool Got(string name, ref ArraySegment<byte> v);

        bool Got<V>(string name, ref V v, byte x = 0xff) where V : IPersist, new();

        bool Got(string name, ref JObj v);

        bool Got(string name, ref JArr v);

        bool Got(string name, ref short[] v);

        bool Got(string name, ref int[] v);

        bool Got(string name, ref long[] v);

        bool Got(string name, ref string[] v);

        bool Got<V>(string name, ref V[] v, byte x = 0xff) where V : IPersist, new();

    }

}