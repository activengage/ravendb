//-----------------------------------------------------------------------
// <copyright file="ITasksStorageActions.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Raven35.Database.Tasks;

namespace Raven35.Database.Storage
{
    using System.Collections.Generic;

    public interface ITasksStorageActions
    {
        void AddTask(DatabaseTask task, DateTime addedAt);
        bool HasTasks { get; }
        long ApproximateTaskCount { get; }

        T GetMergedTask<T>(List<int> indexesToSkip, int[] allIndexes, HashSet<IComparable> alreadySeen)
            where T : DatabaseTask;

        IEnumerable<TaskMetadata> GetPendingTasksForDebug();

        void DeleteTasks(HashSet<IComparable> alreadySeen);

        int DeleteTasksForIndex(int indexId);
    }
}
