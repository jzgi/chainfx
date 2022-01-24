using System;

namespace SkyChain
{
    /// <summary>
    /// Represents a provider or input source of dataset, a data object, or some of its data fields.
    /// </summary>
    public interface ISource : IResource
    {
        bool Get(string name, ref bool v);

        bool Get(string name, ref char v);

        bool Get(string name, ref short v);

        bool Get(string name, ref int v);

        bool Get(string name, ref long v);

        bool Get(string name, ref double v);

        bool Get(string name, ref decimal v);

        bool Get(string name, ref DateTime v);

        bool Get(string name, ref string v);

        bool Get(string name, ref byte[] v);

        bool Get(string name, ref short[] v);

        bool Get(string name, ref int[] v);

        bool Get(string name, ref long[] v);

        bool Get(string name, ref string[] v);

        bool Get(string name, ref JObj v);

        bool Get(string name, ref JArr v);

        bool Get<D>(string name, ref D v, short proj = 0x0fff) where D : IData, new();

        bool Get<D>(string name, ref D[] v, short proj = 0x0fff) where D : IData, new();

        D ToObject<D>(short proj = 0x0fff) where D : IData, new();

        D[] ToArray<D>(short proj = 0x0fff) where D : IData, new();

        /// <summary>
        /// If this input source contains multiple data records.
        /// </summary>
        bool IsDataSet { get; }

        /// <summary>
        /// Move to next data records.
        /// </summary>
        /// <returns>True if sucessfully moved to next data record.</returns>
        bool Next();
    }
}