using System;

namespace ChainFX.Nodal;

public interface IPack<in B>
{
    void Dump(B bdr, DateTime now);

    DateTime Since { get; }
}