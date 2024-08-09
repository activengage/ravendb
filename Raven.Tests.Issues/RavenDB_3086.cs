using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Database.Indexing.IndexMerging;
using Raven35.Tests.Bundles.Authorization.Bugs;
using Raven35.Tests.Bundles.Compression;
using Raven35.Tests.Helpers;
using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_3086 : RavenTestBase
    {
        public class User
        {
            public String Name { get; set; }
            public String Email { get; set; }
            public int Age { get; set; }
        }
        public class UsersByName : AbstractIndexCreationTask<User>
        {
            public UsersByName()
            {
                Map = users => from user in users
                               select new { user.Name };
                Index(x => x.Name, FieldIndexing.Analyzed);

            }
        }

        public class UsersByAge : AbstractIndexCreationTask<User>
        {
            public UsersByAge()
            {
                Map = users => from user in users
                               select new { user.Age };
                Sort(x => x.Age, SortOptions.Int);

            }
        }
        public class UsersByEmail : AbstractIndexCreationTask<User>
        {
            public UsersByEmail()
            {
                Map = users => from user in users
                               select new { user.Email };

            }
        }
      
        [Fact]
        public void IndexMergeWithField()
        {

            using (var store = NewDocumentStore())
            {
                new UsersByName().Execute(store);
                new UsersByEmail().Execute(store);
                new UsersByAge().Execute(store);

                var index1 = store.DatabaseCommands.GetIndex("UsersByName");

                var index2 = store.DatabaseCommands.GetIndex("UsersByEmail");

                var index3 = store.DatabaseCommands.GetIndex("UsersByAge");

                var dictionary = new Dictionary<int, IndexDefinition>()
                {
                    {index1.IndexId, index1},
                    {index2.IndexId, index2},
                    {index3.IndexId, index3}
                };
                IndexMerger merger = new IndexMerger(dictionary);
                IndexMergeResults results = merger.ProposeIndexMergeSuggestions();
                foreach (var suggestion in results.Suggestions)
                {
                    var ind = suggestion.MergedIndex;
                    Assert.Equal(FieldIndexing.Analyzed, ind.Indexes["Name"]);
                    Assert.Equal(SortOptions.Int, ind.SortOptions["Age"]);
                }
            }
        }


    }
}

