// -----------------------------------------------------------------------
//  <copyright file="ReplicationClientConfiguration.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven35.Abstractions.Replication
{
    public class ReplicationClientConfiguration
    {
        public FailoverBehavior? FailoverBehavior { get; set; }
        public bool OnlyModifyFailoverIfNotInClusterAlready { get; set; }
        public double? RequestTimeSlaThresholdInMilliseconds { get; set; }
    }
}
