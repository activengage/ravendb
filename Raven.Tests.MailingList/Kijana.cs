// -----------------------------------------------------------------------
//  <copyright file="Kijana.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;
using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Kijana : RavenTest
    {
        public class Scratch
        {
            public string Id { get; set; }
            public long Value { get; set; }
        }

        class ScratchIndex : AbstractIndexCreationTask<Scratch>
        {
            public ScratchIndex()
            {
                Map = docs =>
                    from doc in docs
                    select new
                    {
                        doc.Value
                    };

                Sort(x => x.Value, SortOptions.Long);
            }
        }

        [Fact]
        public void CanSetSortValue()
        {
            using (var store = NewRemoteDocumentStore())
            {
                new ScratchIndex().Execute(store);
            }
        }
    }
}
