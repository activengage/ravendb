// -----------------------------------------------------------------------
//  <copyright file="TaskMetadata.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven35.Database.Tasks
{
    using System;

    public class TaskMetadata
    {
        public object Id { get; set; }

        public int IndexId { get; set; }

        public string IndexName { get; set; }

        public DateTime AddedTime { get; set; }

        public string Type { get; set; }
    }
}
