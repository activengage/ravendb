// -----------------------------------------------------------------------
//  <copyright file="ScheduledReductionFields.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven35.Database.Storage.Voron.StorageActions.StructureSchemas
{
    public enum ScheduledReductionFields
    {
        IndexId,
        Bucket,
        Level,
        Timestamp,
        ReduceKey,
        Etag
    }
}
