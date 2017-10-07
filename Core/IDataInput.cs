using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents a provider or input source of data record(s).
    /// </summary>
    public interface IDataInput
    {
        bool Get(string name, ref bool v);

        bool Get(string name, ref short v);

        bool Get(string name, ref int v);

        bool Get(string name, ref long v);

        bool Get(string name, ref double v);

        bool Get(string name, ref decimal v);

        bool Get(string name, ref DateTime v);

        bool Get(string name, ref string v);

        bool Get(string name, ref ArraySegment<byte> v);

        bool Get(string name, ref short[] v);

        bool Get(string name, ref int[] v);

        bool Get(string name, ref long[] v);

        bool Get(string name, ref string[] v);

        bool Get(string name, ref Map<string, string> v);

        bool Get<D>(string name, ref D v, short proj = 0x00ff) where D : IData, new();

        bool Get<D>(string name, ref D[] v, short proj = 0x00ff) where D : IData, new();


        IDataInput Let(out bool v);

        IDataInput Let(out short v);

        IDataInput Let(out int v);

        IDataInput Let(out long v);

        IDataInput Let(out double v);

        IDataInput Let(out decimal v);

        IDataInput Let(out DateTime v);

        IDataInput Let(out string v);

        IDataInput Let(out ArraySegment<byte> v);

        IDataInput Let(out short[] v);

        IDataInput Let(out int[] v);

        IDataInput Let(out long[] v);

        IDataInput Let(out string[] v);

        IDataInput Let(out Dictionary<string, string> v);

        IDataInput Let<D>(out D v, short proj = 0x00ff) where D : IData, new();

        IDataInput Let<D>(out D[] v, short proj = 0x00ff) where D : IData, new();

        D ToObject<D>(short proj = 0x00ff) where D : IData, new();

        D[] ToArray<D>(short proj = 0x00ff) where D : IData, new();

        /// <summary>
        /// Write a single (or current) data record into the given output..
        /// </summary>
        /// <param name="o"></param>
        /// <typeparam name="R"></typeparam>
        void Write<R>(IDataOutput<R> o) where R : IDataOutput<R>;

        /// <summary>
        /// Dump as dynamic generated content.
        /// </summary>
        /// <returns></returns>
        DynamicContent Dump();

        /// <summary>
        /// If this input source contains multiple data records.
        /// </summary>
        bool DataSet { get; }

        /// <summary>
        /// Move to next data records.
        /// </summary>
        /// <returns>True if sucessfully moved to next data record.</returns>
        bool Next();
    }
}