using Raven35.Abstractions.Indexing;
using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System.Linq;

namespace Raven35.Tests.Core.Utils.Indexes
{
    public class MultiMapIndex : AbstractMultiMapIndexCreationTask<MultiMapIndex.Result>
    {
        public class Result
        {
            public object[] Content { get; set; }
        }

        public MultiMapIndex()
        {
            AddMap<Company>(items => from x in items
                                     select new Result { Content = new object[] { x.Address1, x.Address2, x.Address3 } });

            AddMap<Headquater>(items => from x in items
                                  select new Result { Content = new object[] { x.Address1, x.Address2, x.Address3 } });

            Index(x => x.Content, FieldIndexing.Analyzed);
        }
    }
}
