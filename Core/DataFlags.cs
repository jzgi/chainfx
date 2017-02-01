using System;

namespace Greatbone.Core
{
    [Flags]
    public enum DataFlags : short
    {
        None = 0,

        All = -1,

        // auto generated or with default
        Auto = 1,

        // binary
        Bin = 2,

        // late-handled
        Late = 4,

        // many
        Sub = 8,

        // hidden or reserved
        Kept = 16
    }
}