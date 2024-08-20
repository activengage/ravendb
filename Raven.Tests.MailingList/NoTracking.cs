// -----------------------------------------------------------------------
//  <copyright file="NoTracking.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Raven35.Client;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.MailingList
{
    public class NoTracking : RavenTest
    {
        private static readonly Guid One = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private static readonly Guid Two = Guid.Parse("00000000-0000-0000-0000-000000000002");

        private IDocumentStore DocumentStore { get; set; }

        public NoTracking()
        {
            DocumentStore = NewDocumentStore();

            using (var session = DocumentStore.OpenSession())
            {
                var a = new A { Id = One };
                var b = new B { Id = Two };
                a.Bs.Add(Two);

                session.Store(a);
                session.Store(b);
                session.SaveChanges();
            }
        }

        public override void Dispose()
        {
            DocumentStore.Dispose();
            base.Dispose();

        }

        [Fact]
        public void Can_load_entities()
        {
            using (var session = DocumentStore.OpenSession())
            {
                Assert.NotNull(session.Load<A>((ValueType)One));
                Assert.NotNull(session.Load<B>((ValueType)Two));
            };
        }

        [Fact]
        public void Can_load_entities_with_NoTracking()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var result = session.Query<A>()
                    .Customize(c => c.NoTracking())
                    .Include<A, B>(a => a.Bs);

                foreach (var res in result)
                {
                    var bs = session.Load<B>(res.Bs.Cast<ValueType>());

                    Assert.Equal(bs.Length, 1);
                    // Fails
                    Assert.NotNull(bs[0]);
                }

                // Doesn't work either, B is null
                Assert.NotNull(session.Load<A>((ValueType)One));
                Assert.NotNull(session.Load<B>((ValueType)Two));
            }
        }

        [Fact]
        public void Can_load_entities_without_NoTrackin()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var result = session.Query<A>()
                    .Include<A, B>(a => a.Bs);

                foreach (var res in result)
                {
                    var bs = session.Load<B>(res.Bs.Cast<ValueType>());

                    Assert.Equal(bs.Length, 1);
                    // Fails
                    Assert.NotNull(bs[0]);
                }

                Assert.NotNull(session.Load<A>((ValueType)One));
                Assert.NotNull(session.Load<B>((ValueType)Two));
            }
        }


        public class A
        {
            public Guid Id { get; set; }
            public ISet<Guid> Bs { get; set; }

            public A()
            {
                this.Bs = new HashSet<Guid>();
            }
        }

        public class B
        {
            public Guid Id { get; set; }
        }
    }
}
