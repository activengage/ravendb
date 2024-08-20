//-----------------------------------------------------------------------
// <copyright file="DatabaseBackupRequest.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Counters;

namespace Raven35.Abstractions.Data
{
    public class CounterStorageBackupRequest
    {
        /// <summary>
        /// Path to directory where backup should lie (must be accessible from server).
        /// </summary>
        public string BackupLocation { get; set; }

        /// <summary>
        /// CounterStorageDocument that will be inserted with backup. If null then document will be taken from server.
        /// </summary>
        public CounterStorageDocument CounterStorageDocument { get; set; }
    }
}
