#if !DNXCORE50
using Raven35.Abstractions.Data;
using Raven35.Client.Document;
using Raven35.Database.Server;
using Raven35.Database.Server.Security.Windows;
using Raven35.Json.Linq;
using Raven35.Tests.Core.Utils.Entities;
using System.Collections.Generic;
using System.Net;
using Raven35.Tests.Common.Attributes;
using Raven35.Tests.Helpers.Util;

using Xunit;

namespace Raven35.Tests.Core.Auth
{
    public class Authentication : RavenCoreTestBase
    {
        [Fact(Skip = "Known failure")]
        public void CanUseApiKeyAuthentication()
        {
            Raven35.Database.Server.Security.Authentication.EnableOnce();
            this.Server.Configuration.AnonymousUserAccessMode = AnonymousUserAccessMode.None;
            this.Server.SystemDatabase.Documents.Put(
                       "Raven/ApiKeys/CanUseApiKeyAuthentication",
                       null,
                       RavenJObject.FromObject(new ApiKeyDefinition
                       {
                           Name = "CanUseApiKeyAuthentication",
                           Secret = "ThisIsMySecret",
                           Enabled = true,
                           Databases = new List<ResourceAccess>
                                {
                                    new ResourceAccess {TenantId = "*"},
                                    new ResourceAccess {TenantId = Constants.SystemDatabase}
                                }
                       }), new RavenJObject(), null);

            using (var store = new DocumentStore
            {
                ApiKey = "CanUseApiKeyAuthentication/ThisIsMySecret2",
                Url = this.Server.SystemDatabase.ServerUrl,
                Credentials = null
            }.Initialize())
            {
                Assert.Throws<Raven35.Abstractions.Connection.ErrorResponseException>(() => store.DatabaseCommands.Get("aa/1"));
            }

            using (var store = new DocumentStore
            {
                ApiKey = "CanUseApiKeyAuthentication/ThisIsMySecret",
                Url = this.Server.SystemDatabase.ServerUrl,
                Credentials = null
            }.Initialize())
            {
                store.DatabaseCommands.Put("users/1", null, RavenJObject.FromObject(new User { }), new RavenJObject());
                var result = store.DatabaseCommands.Get("users/1");
                Assert.NotNull(result);
            }
        }

        [Fact]
        public void CanUseWindowsAuthentication()
        {
            FactIfWindowsAuthenticationIsAvailable.LoadCredentials();
            Raven35.Database.Server.Security.Authentication.EnableOnce();
            this.Server.Configuration.AnonymousUserAccessMode = AnonymousUserAccessMode.None;
            this.Server.SystemDatabase.Documents.Put(
                "Raven/Authorization/WindowsSettings",
                null,
                RavenJObject.FromObject(new WindowsAuthDocument
                {
                    RequiredUsers = new List<WindowsAuthData>
                            {
                                new WindowsAuthData
                                    {
                                        Name = string.Format("{0}\\{1}", FactIfWindowsAuthenticationIsAvailable.Admin.Domain, FactIfWindowsAuthenticationIsAvailable.Admin.UserName),
                                        Enabled = true,
                                        Databases = new List<ResourceAccess>
                                            {
                                                new ResourceAccess {TenantId = "*"},
                                                new ResourceAccess {TenantId = Constants.SystemDatabase}
                                            }
                                    }
                            }
                }), new RavenJObject(), null);

            using (var store = new DocumentStore
            {
                Credentials = new NetworkCredential(FactIfWindowsAuthenticationIsAvailable.User.UserName, FactIfWindowsAuthenticationIsAvailable.User.Password, FactIfWindowsAuthenticationIsAvailable.User.Domain),
                Url = this.Server.SystemDatabase.ServerUrl
            })
            {
                ConfigurationHelper.ApplySettingsToConventions(store.Conventions);

                store.Initialize();
                Assert.Throws<Raven35.Abstractions.Connection.ErrorResponseException>(() => store.DatabaseCommands.Put("users/1", null, RavenJObject.FromObject(new User { }), new RavenJObject()));
            }

            using (var store = new DocumentStore
            {
                Credentials = new NetworkCredential(FactIfWindowsAuthenticationIsAvailable.Admin.UserName, FactIfWindowsAuthenticationIsAvailable.Admin.Password, FactIfWindowsAuthenticationIsAvailable.Admin.Domain),
                Url = this.Server.SystemDatabase.ServerUrl
            })
            {
                ConfigurationHelper.ApplySettingsToConventions(store.Conventions);

                store.Initialize();

                store.DatabaseCommands.Put("users/1", null, RavenJObject.FromObject(new User { }), new RavenJObject());
                var result = store.DatabaseCommands.Get("users/1");
                Assert.NotNull(result);
            }
        }
    }
}
#endif