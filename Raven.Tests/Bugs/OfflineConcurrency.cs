using System;
using Raven35.Abstractions.Exceptions;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class OfflineConcurrency : RavenTest
    {
        [Fact]
        public void Successful()
        {
            Raven35.Abstractions.Data.Etag guid;

            using(var store = NewDocumentStore())
            {
                using(var session = store.OpenSession())
                {
                    var entity = new User{Id = "users/1"};
                    session.Store(entity);
                    session.SaveChanges();

                    guid = session.Advanced.GetEtagFor(entity);
                }

                using (var session = store.OpenSession())
                {
                    session.Advanced.UseOptimisticConcurrency = true; 
                    var entity = new User { Id = "users/1" };
                    session.Store(entity, guid);
                    session.SaveChanges();
                }
            }
        }

        [Fact]
        public void Failed()
        {
            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    var entity = new User { Id = "users/1" }; session.Store(entity);
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    session.Advanced.UseOptimisticConcurrency = true;
                    var entity = new User { Id = "users/1" }; 
                    session.Store(entity,Raven35.Abstractions.Data.Etag.InvalidEtag);
                    Assert.Throws<ConcurrencyException>(() => session.SaveChanges());
                }
            }
        }
    }
}
