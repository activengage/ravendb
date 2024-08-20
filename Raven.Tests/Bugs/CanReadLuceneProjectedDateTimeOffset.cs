using System;
using System.IO;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Client.Document;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class CanReadLuceneProjectedDateTimeOffset : NoDisposalNeeded
    {
        [Fact]
        public void Can_read_date_time_offset_from_lucene_query()
        {
            var jsonSerializer = new DocumentConvention().CreateSerializer();

            using (var reader = new JsonTextReader(new StringReader(@"{""Item"": ""20090402193554412""}")))
            {
                var deserialize = jsonSerializer.Deserialize<Test>(reader);
                Assert.Equal(2009, deserialize.Item.Year);
            }
        }

        private class Test
        {
            public DateTimeOffset Item { get; set; }
        }
    }
}
