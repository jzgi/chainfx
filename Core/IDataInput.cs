using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// Represents A DAT source for load.
    ///
    public interface IDataInput
    {
        bool Get(string name, ref bool v);

        bool Get(string name, ref short v);

        bool Get(string name, ref int v);

        bool Get(string name, ref long v);

        bool Get(string name, ref double v);

        bool Get(string name, ref decimal v);

        bool Get(string name, ref DateTime v);

        bool Get(string name, ref NpgsqlPoint v);

        bool Get(string name, ref char[] v);

        bool Get(string name, ref string v);

        bool Get(string name, ref byte[] v);

        bool Get(string name, ref ArraySegment<byte> v);

        bool Get<D>(string name, ref D v, byte flags = 0) where D : IData, new();

        bool Get(string name, ref short[] v);

        bool Get(string name, ref int[] v);

        bool Get(string name, ref long[] v);

        bool Get(string name, ref string[] v);

        bool Get<D>(string name, ref D[] v, byte flags = 0) where D : IData, new();

        bool Get<D>(string name, ref List<D> v, byte flags = 0) where D : IData, new();

        D ToObject<D>(byte flags = 0) where D : IData, new();

        ///
        /// Write a single (or current) data entry into the given output object.
        ///
        void WriteData<R>(IDataOutput<R> o) where R : IDataOutput<R>;

        ///
        /// Dump as specified content.
        ///
        C Dump<C>() where C : IContent, IDataOutput<C>, new();

        ///
        /// If this model includes multiple data entries.
        ///
        bool DataSet { get; }

        ///
        /// Move to next data entry.
        ///
        bool Next();

        D[] ToArray<D>(byte flags = 0) where D : IData, new();

        List<D> ToList<D>(byte flags = 0) where D : IData, new();
    }
}