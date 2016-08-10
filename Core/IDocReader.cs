using System.Collections.Generic;

namespace Greatbone.Core
{
    public interface IDocReader
    {
        bool ReadStart();

        bool ReadEnd();

        bool Read(string name, ref int value);

        bool Read(string name, ref decimal value);

        bool Read(string name, ref string value);

        bool Read<T>(string name, ref List<T> value) where T : IDoc;
    }
}