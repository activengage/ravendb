// -----------------------------------------------------------------------
//  <copyright file="AbstractSynchronizationTrigger.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel.Composition;
using Raven35.Abstractions.FileSystem;
using Raven35.Json.Linq;

namespace Raven35.Database.FileSystem.Plugins
{
    [InheritedExport]
    public abstract class AbstractSynchronizationTrigger: IRequiresFileSystemInitialization
    {
        public RavenFileSystem FileSystem { get; private set; }

        public virtual void Initialize(RavenFileSystem fileSystem)
        {
            FileSystem = fileSystem;
            Initialize();
        }

        public virtual void Initialize()
        {
        }

        public virtual void SecondStageInit()
        {
        }

        public virtual void BeforeSynchronization(string name, RavenJObject metadata, SynchronizationType type)
        {
        }

        public virtual void AfterSynchronization(string name, RavenJObject metadata, SynchronizationType type, dynamic additionalData)
        {
        }
    }
}
