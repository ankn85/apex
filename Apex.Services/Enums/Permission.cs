using System;

namespace Apex.Services.Enums
{
    [Flags]
    public enum Permission
    {
        None = 0,
        Create = 1 << 0,
        View = 1 << 1,
        Update = 1 << 2,
        Delete = 1 << 3,
        // Combinations.
        ViewCreate = View ^ Create,
        ViewUpdate = View ^ Update,
        ViewDelete = View ^ Delete,
        Full = View ^ Create ^ Update ^ Delete
    }
}
