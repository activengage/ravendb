using System;
using System.Collections.Generic;
using System.Linq;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class linmouhong3 : RavenTest
    {
        public class ShortUrlMap
        {
            public string LongUrl { get; set; }

            public string ShortUrl { get; set; }
        }

        public class ShortUrlMapIndex : AbstractIndexCreationTask<ShortUrlMap>
        {
            public ShortUrlMapIndex()
            {
                Map = maps => from m in maps
                              select new
                              {
                                  m.ShortUrl,
                                  m.LongUrl
                              };
            }
        }

        [Fact]
        public void InQueriesWork()
        {
            using (var Database = NewDocumentStore())
            {
                new ShortUrlMapIndex().Execute(Database);

                using (var session = Database.OpenSession())
                {
                    session.Store(new ShortUrlMap
                    {
                        LongUrl = "http://www.a.com",
                        ShortUrl = "http://t.cn/abc"
                    });
                    session.Store(new ShortUrlMap
                    {
                        LongUrl = "http://www.abcdef-134234.com",
                        ShortUrl = "http://t.cn/def"
                    });
                    session.SaveChanges();
                }


                using (var session = Database.OpenSession())
                {
                    var longUrls = new List<string>
                    {
                        "http://www.a.com",
                        "http://ctow.sigcms.com/click?"
                    };

                    var query1 = session.Query<ShortUrlMap, ShortUrlMapIndex>()
                                        .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                                        .Where(x => x.LongUrl.In(longUrls))
                                        .ToList();
                    Assert.Equal(1, query1.Count);

                }

            }
        }

    }
}
