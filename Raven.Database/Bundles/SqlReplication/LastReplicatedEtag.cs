using System;
using Raven35.Abstractions.Data;

namespace Raven35.Database.Bundles.SqlReplication
{
    public class LastReplicatedEtag
    {
        public string Name { get; set; }
        public Etag LastDocEtag { get; set; }
    }
}
