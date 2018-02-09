using System;

namespace Greatbone.Core
{
    public interface IDbParams : IDataOutput<IDbParams>
    {
        IDbParams SetNull();

        IDbParams Set(IDataInput v);

        IDbParams Set(bool v);

        IDbParams Set(short v);

        IDbParams Set(int v);

        IDbParams Set(long v);

        IDbParams Set(double v);

        IDbParams Set(decimal v);

        IDbParams Set(JNumber v);

        IDbParams Set(DateTime v);

        IDbParams Set(string v);

        IDbParams Set(ArraySegment<byte> v);

        IDbParams Set(short[] v);

        IDbParams Set(int[] v);

        IDbParams Set(long[] v);

        IDbParams Set(string[] v);

        IDbParams Set(IData v, byte proj = 0x0f);

        IDbParams Set<D>(D[] v, byte proj = 0x0f) where D : IData;
    }
}