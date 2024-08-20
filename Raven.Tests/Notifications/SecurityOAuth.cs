// -----------------------------------------------------------------------
//  <copyright file="Security.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Lucene.Net.Util;
using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Replication;
using Raven35.Client.Document;
using Raven35.Database.Server;
using Raven35.Database.Server.Security;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Notifications
{
    public class SecurityOAuth : RavenTest
    {
        protected override void ModifyConfiguration(Database.Config.InMemoryRavenConfiguration configuration)
        {
            configuration.AnonymousUserAccessMode = AnonymousUserAccessMode.None;
            Authentication.EnableOnce();
            configuration.PostInit();
        }


        protected override void CreateDefaultIndexes(Client.IDocumentStore documentStore)
        {
        }

        [Fact]
        public void WithOAuthOnSystemDatabase()
        {
            using (var server = GetNewServer(enableAuthentication:true))
            {
                server.SystemDatabase.Documents.Put("Raven/ApiKeys/test", null, RavenJObject.FromObject(new ApiKeyDefinition
                {
                    Name = "test",
                    Secret = "test",
                    Enabled = true,
                    Databases = new List<ResourceAccess>
                    {
                        new ResourceAccess {TenantId = "<system>"},
                    }
                }), new RavenJObject(), null);

                using (var store = new DocumentStore
                {
                    ApiKey = "test/test",
                    Url = "http://localhost:8079",
                    Conventions = { FailoverBehavior = FailoverBehavior.FailImmediately }
                }.Initialize())
                {
                    var list = new BlockingCollection<DocumentChangeNotification>();
                    var taskObservable = store.Changes();
                    taskObservable.Task.Wait();
                    var documentSubscription = taskObservable.ForDocument("items/1");
                    documentSubscription.Task.Wait();
                    documentSubscription
                        .Subscribe(list.Add);

                    using (var session = store.OpenSession())
                    {
                        session.Store(new ClientServer.Item(), "items/1");
                        session.SaveChanges();
                    }

                    DocumentChangeNotification changeNotification;
                    Assert.True(list.TryTake(out changeNotification, TimeSpan.FromSeconds(2)));

                    Assert.Equal("items/1", changeNotification.Id);
                    Assert.Equal(changeNotification.Type, DocumentChangeTypes.Put);
                }
            }
        }

        [Fact]
        public void WithOAuthWrongKeyFails()
        {
            using (var server = GetNewServer(enableAuthentication:true))
            {
                server.SystemDatabase.Documents.Put("Raven/ApiKeys/test", null, RavenJObject.FromObject(new ApiKeyDefinition
                {
                    Name = "test",
                    Secret = "test",
                    Enabled = true,
                    Databases = new List<ResourceAccess>
                    {
                        new ResourceAccess {TenantId = "*"},
                    }
                }), new RavenJObject(), null);

                using (var store = new DocumentStore
                {
                    ApiKey = "NotRealKeys",
                    Url = "http://localhost:8079",
                    Conventions = { FailoverBehavior = FailoverBehavior.FailImmediately }
                }.Initialize())
                {
                    var exception = Assert.Throws<InvalidOperationException>(() =>
                    {
                        using (var session = store.OpenSession())
                        {
                            session.Store(new ClientServer.Item(), "items/1");
                            session.SaveChanges();
                        }
                    });
                    Assert.Equal("Invalid API key", exception.Message);
                }
            }
        }

        [Fact]
        public void WithOAuthOnSpecificDatabase()
        {
            using (var server = GetNewServer(enableAuthentication:true))
            {
                server.SystemDatabase.Documents.Put("Raven35.Databases/OAuthTest", null, RavenJObject.FromObject(new DatabaseDocument
                {
                    Disabled = false,
                    Id = "Raven35.Databases/OAuthTest",
                    Settings = new IdentityDictionary<string, string>
                    {
                        {"Raven/DataDir", "~\\Databases\\OAuthTest"}
                    }
                }), new RavenJObject(), null);

                server.SystemDatabase.Documents.Put("Raven/ApiKeys/test", null, RavenJObject.FromObject(new ApiKeyDefinition
                {
                    Name = "test",
                    Secret = "test",
                    Enabled = true,
                    Databases = new List<ResourceAccess>
                    {
                        new ResourceAccess {TenantId = "OAuthTest"},
                    }
                }), new RavenJObject(), null);

                using (var store = new DocumentStore
                {
                    ApiKey = "test/test",
                    DefaultDatabase = "OAuthTest",
                    Url = "http://localhost:8079",
                    Conventions = { FailoverBehavior = FailoverBehavior.FailImmediately }
                }.Initialize())
                {
                    var list = new BlockingCollection<DocumentChangeNotification>();
                    var taskObservable = store.Changes();
                    taskObservable.Task.Wait();
                    var documentSubscription = taskObservable.ForDocument("items/1");
                    documentSubscription.Task.Wait();
                    documentSubscription
                        .Subscribe(list.Add);

                    using (var session = store.OpenSession())
                    {
                        session.Store(new ClientServer.Item(), "items/1");
                        session.SaveChanges();
                    }

                    DocumentChangeNotification changeNotification;
                    Assert.True(list.TryTake(out changeNotification, TimeSpan.FromSeconds(2)));

                    Assert.Equal("items/1", changeNotification.Id);
                    Assert.Equal(changeNotification.Type, DocumentChangeTypes.Put);
                }
            }
        }

        [Fact]
        public void WithOAuthOnSpecificDatabaseWontWorkForAnother()
        {
            using (var server = GetNewServer(enableAuthentication:true))
            {
                server.SystemDatabase.Documents.Put("Raven35.Databases/OAuthTest1", null, RavenJObject.FromObject(new DatabaseDocument
                {
                    Disabled = false,
                    Id = "Raven35.Databases/OAuthTest1",
                    Settings = new IdentityDictionary<string, string>
                    {
                        {"Raven/DataDir", "~\\Databases\\OAuthTest1"}
                    }
                }), new RavenJObject(), null);

                server.SystemDatabase.Documents.Put("Raven35.Databases/OAuthTest2", null, RavenJObject.FromObject(new DatabaseDocument
                {
                    Disabled = false,
                    Id = "Raven35.Databases/OAuthTest2",
                    Settings = new IdentityDictionary<string, string>
                    {
                        {"Raven/DataDir", "~\\Databases\\OAuthTest2"}
                    }
                }), new RavenJObject(), null);

                server.SystemDatabase.Documents.Put("Raven/ApiKeys/test", null, RavenJObject.FromObject(new ApiKeyDefinition
                {
                    Name = "test",
                    Secret = "test",
                    Enabled = true,
                    Databases = new List<ResourceAccess>
                    {
                        new ResourceAccess {TenantId = "OAuthTest1"},
                    }
                }), new RavenJObject(), null);

                using (var store = new DocumentStore
                {
                    ApiKey = "test/test",
                    DefaultDatabase = "OAuthTest2",
                    Url = "http://localhost:8079",
                    Conventions = { FailoverBehavior = FailoverBehavior.FailImmediately }
                }.Initialize())
                {
                    Assert.Throws<ErrorResponseException>(() =>
                    {
                        using (var session = store.OpenSession())
                        {
                            session.Store(new ClientServer.Item(), "items/1");
                            session.SaveChanges();
                        }
                    });
                }
            }
        }

        [Fact]
        public void WithOAuthWithStarWorksForAnyDatabaseOtherThenSystem()
        {
            using (var server = GetNewServer(enableAuthentication:true))
            {
                server.SystemDatabase.Documents.Put("Raven35.Databases/OAuthTest", null, RavenJObject.FromObject(new DatabaseDocument
                {
                    Disabled = false,
                    Id = "Raven35.Databases/OAuthTest",
                    Settings = new IdentityDictionary<string, string>
                    {
                        {"Raven/DataDir", "~\\Databases\\OAuthTest"}
                    }
                }), new RavenJObject(), null);

                server.SystemDatabase.Documents.Put("Raven/ApiKeys/test", null, RavenJObject.FromObject(new ApiKeyDefinition
                {
                    Name = "test",
                    Secret = "test",
                    Enabled = true,
                    Databases = new List<ResourceAccess>
                    {
                        new ResourceAccess {TenantId = "*"},
                    }
                }), new RavenJObject(), null);

                using (var store = new DocumentStore
                {
                    ApiKey = "test/test",
                    DefaultDatabase = "OAuthTest",
                    Url = "http://localhost:8079",
                    Conventions = { FailoverBehavior = FailoverBehavior.FailImmediately }
                }.Initialize())
                {
                    var list = new BlockingCollection<DocumentChangeNotification>();
                    var taskObservable = store.Changes();
                    taskObservable.Task.Wait();
                    var documentSubscription = taskObservable.ForDocument("items/1");
                    documentSubscription.Task.Wait();
                    documentSubscription
                        .Subscribe(list.Add);

                    using (var session = store.OpenSession())
                    {
                        session.Store(new ClientServer.Item(), "items/1");
                        session.SaveChanges();
                    }

                    DocumentChangeNotification changeNotification;
                    Assert.True(list.TryTake(out changeNotification, TimeSpan.FromSeconds(2)));

                    Assert.Equal("items/1", changeNotification.Id);
                    Assert.Equal(changeNotification.Type, DocumentChangeTypes.Put);
                }

                using (var store = new DocumentStore
                {
                    ApiKey = "test/test",
                    Url = "http://localhost:8079",
                    Conventions = { FailoverBehavior = FailoverBehavior.FailImmediately }
                }.Initialize())
                {
                    Assert.Throws<ErrorResponseException>(() =>
                    {
                        using (var session = store.OpenSession())
                        {
                            session.Store(new ClientServer.Item(), "items/1");
                            session.SaveChanges();
                        }
                    });
                }
            }
        }
    }
}
