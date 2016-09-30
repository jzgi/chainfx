using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data outputing destination.
    /// </summary>
    public interface IParameters : IDataOut<IParameters>
    {
        IParameters Put(bool value);

        IParameters Put(short value);

        IParameters Put(int value);
    }
}