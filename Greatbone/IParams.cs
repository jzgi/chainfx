using System;

namespace Greatbone
{
    /// <summary>
    /// To set SQL parameters.  
    /// </summary>
    public interface IParams : ISink
    {
        IParams SetNull();

        IParams Set(bool v);

        IParams Set(short v);

        IParams Set(int v);

        IParams Set(long v);

        IParams Set(double v);

        IParams Set(decimal v);

        IParams Set(JNumber v);

        IParams Set(DateTime v);

        IParams Set(string v);

        IParams Set(ArraySegment<byte> v);

        IParams Set(byte[] v);

        IParams Set(short[] v);

        IParams Set(int[] v);

        IParams Set(long[] v);

        IParams Set(string[] v);

        IParams Set(JObj v);

        IParams Set(JArr v);

        IParams Set(IData v, byte proj = 0x0f);

        IParams Set<D>(D[] v, byte proj = 0x0f) where D : IData;

        IParams SetIn(string[] v);
    }
}