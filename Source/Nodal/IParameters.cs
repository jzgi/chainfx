using System;
using System.Collections.Generic;

namespace ChainFx.Nodal
{
    /// <summary>
    /// To set SQL parameters.  
    /// </summary>
    public interface IParameters : ISink
    {
        IParameters SetNull();

        IParameters Set(bool v);

        IParameters Set(char v);

        IParameters Set(byte v);

        IParameters Set(short v);

        IParameters SetOrNull(short v);

        IParameters Set(int v);

        IParameters SetOrNull(int v);

        IParameters Set(long v);

        IParameters SetOrNull(long v);

        IParameters Set(float v);

        IParameters Set(double v);

        IParameters Set(decimal v);

        IParameters Set(JNumber v);

        IParameters Set(DateTime v);

        IParameters Set(TimeSpan v);

        IParameters Set(string v);

        IParameters Set(bool[] v);

        IParameters Set(char[] v);

        IParameters Set(byte[] v);

        IParameters Set(ArraySegment<byte> v);

        IParameters Set(short[] v);

        IParameters Set(int[] v);

        IParameters Set(long[] v);

        IParameters Set(float[] v);

        IParameters Set(double[] v);

        IParameters Set(DateTime[] v);

        IParameters Set(string[] v);

        IParameters Set(JObj v);

        IParameters Set(JArr v);

        IParameters Set(IData v, short msk = 0xff);

        IParameters Set<D>(D[] v, short msk = 0xff) where D : IData;

        IParameters SetForIn(short[] v);

        IParameters SetForIn(int[] v);

        IParameters SetForIn<M>(IList<M> v) where M : IKeyable<int>;

        IParameters SetForIn(long[] v);

        IParameters SetForIn(DateTime[] v);

        IParameters SetForIn(string[] v);

        IParameters SetMoment();
    }
}