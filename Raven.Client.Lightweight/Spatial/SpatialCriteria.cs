using Raven35.Abstractions.Indexing;

namespace Raven35.Client.Spatial
{
    public class SpatialCriteria
    {
        public SpatialRelation Relation { get; set; }
        public object Shape { get; set; }
        public double DistanceErrorPct { get; set; }
    }
}
