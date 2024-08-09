using System;

namespace Raven35.Abstractions.Data
{
    public enum FacetMode
    {
        Default,
        Ranges,
    }

    [Flags]
    public enum FacetAggregation 
    {
        None = 0,
        Count = 1,
        Max = 2,
        Min = 4,
        Average = 8,
        Sum = 16,
        Distinct = 32
    }
}
