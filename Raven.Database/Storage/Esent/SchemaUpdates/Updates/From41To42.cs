// -----------------------------------------------------------------------
//  <copyright file="From40To41.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using Raven35.Database.Config;
using Raven35.Database.Impl;
using Raven35.Abstractions.Extensions;
using Raven35.Database.Storage;
using Raven35.Storage.Esent.StorageActions;

namespace Raven35.Storage.Esent.SchemaUpdates.Updates
{
    public class From41To42 : ISchemaUpdate
    {
        public string FromSchemaVersion { get { return "4.1"; } }

        public void Init(IUuidGenerator generator, InMemoryRavenConfiguration configuration)
        {

        }

        public void Update(Session session, JET_DBID dbid, Action<string> output)
        {
            var defaultVal = BitConverter.GetBytes(0);
            Api.JetSetColumnDefaultValue(session, dbid, "reduce_keys_status", "reduce_type", defaultVal, defaultVal.Length,
                                         SetColumnDefaultValueGrbit.None);

            SchemaCreator.UpdateVersion(session, dbid, "4.2");
        }
    }
}
