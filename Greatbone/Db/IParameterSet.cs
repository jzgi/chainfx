using System;

namespace Greatbone.Db
{
    /// <summary>
    /// To set SQL parameters.  
    /// </summary>
    public interface IParameterSet : ISink
    {
        IParameterSet SetNull();

        IParameterSet Set(bool v);

        IParameterSet Set(char v);

        IParameterSet Set(byte v);

        IParameterSet Set(short v);

        IParameterSet Set(int v);

        IParameterSet Set(long v);

        IParameterSet Set(uint v);

        IParameterSet Set(float v);

        IParameterSet Set(double v);

        IParameterSet Set(decimal v);

        IParameterSet Set(JNumber v);

        IParameterSet Set(DateTime v);

        IParameterSet Set(Guid v);

        IParameterSet Set(string v);

        IParameterSet Set(bool[] v);

        IParameterSet Set(char[] v);

        IParameterSet Set(byte[] v);

        IParameterSet Set(ArraySegment<byte> v);

        IParameterSet Set(short[] v);

        IParameterSet Set(int[] v);

        IParameterSet Set(long[] v);

        IParameterSet Set(uint[] v);

        IParameterSet Set(float[] v);

        IParameterSet Set(double[] v);

        IParameterSet Set(DateTime[] v);

        IParameterSet Set(Guid[] v);

        IParameterSet Set(string[] v);

        IParameterSet Set(JObj v);

        IParameterSet Set(JArr v);

        IParameterSet Set(IData v, byte proj = 0x0f);

        IParameterSet Set<D>(D[] v, byte proj = 0x0f) where D : IData;

        IParameterSet SetIn(string[] v);

        IParameterSet SetIn(short[] v);

        IParameterSet SetIn(int[] v);

        IParameterSet SetIn(long[] v);
    }
}