// -----------------------------------------------------------------------
//  <copyright file="TypeConverter.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class TypeConverter : RavenTest
    {
        [Fact]
        public void ShouldWork()
        {
            var test = new Test {Name = "Hello World"};

            using (IDocumentStore documentStore = NewDocumentStore())
            {
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    session.Store(test);

                    session.SaveChanges();
                }
            }
        }

        [JsonObject]
        [TypeConverter(typeof (ExpandableObjectConverter))]
        public class Test
        {
            /// <summary>Gets or sets the Name.</summary>
            public string Name { get; set; }

            /// <summary>The to string.</summary>
            /// <returns>
            ///     The <see cref="string" />Name.
            /// </returns>
            public override string ToString()
            {
                return Name;
            }
        }
    }
}

