//-----------------------------------------------------------------------
// <copyright file="FailStore.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Raven35.Client;
using Raven35.Client.Listeners;
using Raven35.Json.Linq;

namespace Raven35.Tests.Bugs
{
    public class FailStore : IDocumentStoreListener
    {
        public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
        {
            throw new NotImplementedException();
        }

        public void AfterStore(string key, object entityInstance, RavenJObject metadata)
        {
            throw new NotImplementedException();
        }
    }
}
