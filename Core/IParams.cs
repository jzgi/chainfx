using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To set SQL parameters.  
    /// </summary>
    public interface IParams : ISink
    {
        IParams SetNull();

        IParams Set(ISource v);

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

        IParams Set(short[] v);

        IParams Set(int[] v);

        IParams Set(long[] v);

        IParams Set(string[] v);

        IParams Set(IData v, byte proj = 0x0f);

        IParams Set<D>(D[] v, byte proj = 0x0f) where D : IData;
    }
}