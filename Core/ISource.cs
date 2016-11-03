using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents A source for object persistence.
    /// </summary>
    public interface ISource
    {

        bool Get(string name, ref bool v);

        bool Get(string name, ref short v);

        bool Get(string name, ref int v);

        bool Get(string name, ref long v);

        bool Get(string name, ref decimal v);

        bool Get(string name, ref Number v);

        bool Get(string name, ref DateTime v);

        bool Get(string name, ref char[] v);

        bool Get(string name, ref string v);

        bool Get(string name, ref byte[] v);

        bool Get(string name, ref ArraySegment<byte>? v);

        bool Get<V>(string name, ref V v, byte x = 0) where V : IPersist, new();

        bool Get(string name, ref JObj v);

        bool Get(string name, ref JArr v);

        bool Get(string name, ref short[] v);

        bool Get(string name, ref int[] v);

        bool Get(string name, ref long[] v);

        bool Get(string name, ref string[] v);

        bool Get<V>(string name, ref V[] v, byte x = 0) where V : IPersist, new();

    }

}