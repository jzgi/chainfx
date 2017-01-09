﻿namespace Greatbone.Core
{
    ///
    /// A context that can initiate remote call.
    ///
    public interface ICaller
    {
        IToken Token { get; }

        string TokenStr { get; }

        bool Cookied { get; }
    }
}