using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Map<D> : Dictionary<string, D> where D : IData
    {
    }
}