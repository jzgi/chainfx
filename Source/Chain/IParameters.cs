using System;
using SkyChain;

namespace SkyChain.Chain
{
    /// <summary>
    /// To set SQL parameters.  
    /// </summary>
    public interface IParameters : ISink
    {
        bool Digest { get; set; }

        long Checksum { get; }

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

        IParameters Set(IData v, short proj = 0x0fff);

        IParameters Set<D>(D[] v, short proj = 0x0fff) where D : IData;

        IParameters SetForIn(short[] v);

        IParameters SetForIn(int[] v);

        IParameters SetForIn(long[] v);

        IParameters SetForIn(DateTime[] v);

        IParameters SetForIn(string[] v);
        
        IParameters SetMoment();
    }
}