using System;

namespace Greatbone.Core
{
    ///
    /// To mark a variable-name folder.
    ///
    public interface IVar
    {
        // to get variable key from token
        Func<IData, string> Keyer { get; }
    }
}