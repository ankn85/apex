using System;

namespace Apex.Services.Enums
{
    [Flags]
    public enum Permission
    {
        None = 0,
        Read = 1 << 0,
        Host = 1 << 1,
        // Combinations.
        Full = Read ^ Host
    }
}
