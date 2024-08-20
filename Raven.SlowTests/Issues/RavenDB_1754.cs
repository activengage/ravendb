// -----------------------------------------------------------------------
//  <copyright file="RavenDB_1754.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Raven35.Abstractions.Commands;
using Raven35.Abstractions.Data;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.SlowTests.Issues
{
    public class RavenDB_1754 : RavenTest
    {
        
        [Fact]
        public void ShouldntThrowCollectionModified()
        {
            using (var store = NewDocumentStore(requestedStorage: "esent"))
            {
                var o = new Test { Slugs = { { "FOO", "bar" } } };
                using (var session = store.OpenSession())
                {
                    session.Store(o);
                    session.SaveChanges();
                }
                Parallel.For(0, 10000, _ =>
                {
                    using (var session = store.OpenSession())
                    {
                        session.Advanced.UseOptimisticConcurrency = true;
                        session.Advanced.Defer(
                        new PatchCommandData
                        {
                            Key = o.Id,
                            Patches = new[] {
                                new PatchRequest 
                                { 
                                    Type = PatchCommandType.Modify, 
                                    Name = "Slugs", 
                                    Nested = new[] {
                                        new PatchRequest {
                                            Type = PatchCommandType.Set,
                                            Name = "two",
                                            Value = "one" 
                                        }
                                    }
                                }

                            }
                        });
                        session.SaveChanges();
                    }
                });
            }
        }

        public class Test
        {
            public string Id { get; set; }
            public Dictionary<string, string> Slugs { get; set; }
            public Test()
            {
                Slugs = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            }
        }
    }
}
