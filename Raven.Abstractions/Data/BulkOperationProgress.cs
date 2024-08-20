// -----------------------------------------------------------------------
//  <copyright file="BulkOperationProgress.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven35.Abstractions.Data
{
    public class BulkOperationProgress
    {
        public int TotalEntries;

        public int ProcessedEntries;
    }
}