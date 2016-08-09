using System.Collections.Generic;

namespace Greatbone.Core
{
    public interface IDataInput
    {
        bool GotStart();

        bool GotEnd();

        bool Got(string name, ref int value);

        bool Got(string name, ref decimal value);

        bool Got(string name, ref string value);

        bool Got<T>(string name, ref List<T> value) where T : IData;
    }
}