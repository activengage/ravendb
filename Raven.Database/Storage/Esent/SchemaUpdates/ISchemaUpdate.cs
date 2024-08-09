//-----------------------------------------------------------------------
// <copyright file="ISchemaUpdate.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using Microsoft.Isam.Esent.Interop;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Database.Impl;

namespace Raven35.Storage.Esent.SchemaUpdates
{
    [InheritedExport]
    public interface ISchemaUpdate
    {
        string FromSchemaVersion { get;  }
        void Init(IUuidGenerator generator, InMemoryRavenConfiguration configuration);
        void Update(Session session, JET_DBID dbid, Action<string> output);
    }
}
