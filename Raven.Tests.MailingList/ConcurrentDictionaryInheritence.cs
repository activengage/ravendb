// -----------------------------------------------------------------------
//  <copyright file="ConcurrentDictionaryInheritence.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using Raven35.Client.Document;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Imports.Newtonsoft.Json.Serialization;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.MailingList
{
    public class ConcurrentDictionaryInheritence : RavenTest
    {
        public class Countries : ConcurrentDictionary<string, string>
        {
        }

        public class MyContractResolver : DefaultRavenContractResolver
        {
            public MyContractResolver(bool shareCache) : base(shareCache)
            {
            }

            protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
            {
                var jsonDictionaryContract = base.CreateDictionaryContract(objectType);
                if (objectType == typeof (Countries))
                {
                    jsonDictionaryContract.OnDeserializedCallbacks.Clear();
                }
                return jsonDictionaryContract;
            }
        }

        [Fact]
        public void ShouldBeAbleToSaveAndRetrieveCountries()
        {
            using (var documentStore = NewDocumentStore())
            {
                documentStore.Conventions.CustomizeJsonSerializer += serializer =>
                {
                    serializer.ContractResolver = new MyContractResolver(true);
                };
                var document = new Countries();
                document["Canada"] = "Canada";

                using (var session = documentStore.OpenSession())
                {
                    session.Store(document, "Countries");
                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    session.Load<Countries>("Countries");
                }
            }
        } 
    }
}
