// -----------------------------------------------------------------------
//  <copyright file="GlobalReplicationTopologyRequest.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven35.Database.Bundles.Replication.Data
{
    public class GlobalReplicationTopologyRequest
    {
        public bool Databases { get; set; }
        public bool Filesystems { get; set; }
        public bool Counters { get; set; }
    }
}
