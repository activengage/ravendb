using System;

namespace Raven35.Abstractions.FileSystem
{
    [Flags]
    public enum FilesSortOptions
    {
        Default = 0,
        Name = 1,
        Size = 2,
        LastModified = 8,

        Desc = 1024
    }
}
