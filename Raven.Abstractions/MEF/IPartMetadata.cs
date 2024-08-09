using System;
using System.ComponentModel;

namespace Raven35.Abstractions.MEF
{
    public interface IPartMetadata
    {
        [DefaultValue(0)]
        int Order { get; }
    }
}
