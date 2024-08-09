using Raven35.Abstractions;
using Raven35.Client;
using Raven35.Tests.Common;

namespace Raven35.Tests.Bugs.LiveProjections
{
    using System;
    using System.Linq;
    using Raven35.Client.Linq;
    using Raven35.Tests.Bugs.LiveProjections.Entities;
    using Raven35.Tests.Bugs.LiveProjections.Indexes;
    using Raven35.Tests.Bugs.LiveProjections.Views;
    using Xunit;

    public class LiveProjectionOnTasks : RavenTest
    {
        [Fact]
        public void TaskLiveProjection()
        {
            using (var documentStore = NewDocumentStore())
            {
                new TaskSummaryIndex().Execute(((IDocumentStore)documentStore).DatabaseCommands, ((IDocumentStore)documentStore).Conventions);
                new TaskSummaryTransformer().Execute(documentStore);

                using (var session = documentStore.OpenSession())
                {
                    session.Store(
                        new User() { Name = "John Doe" }
                    );

                    session.Store(
                        new User() { Name = "Bob Smith" }
                    );

                    session.Store(
                        new Place() { Name = "Coffee Shop" }
                    );

                    session.Store(
                        new Task()
                        {
                            Description = "Testing projections",
                            Start = SystemTime.UtcNow,
                            End = SystemTime.UtcNow.AddMinutes(30),
                            GiverId = 1,
                            TakerId = 2,
                            PlaceId = 1
                        }
                    );

                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    var results = session.Query<dynamic, TaskSummaryIndex>()
                        .TransformWith<TaskSummaryTransformer, dynamic>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                        .As<TaskSummary>()
                        .ToList();

                    var first = results.FirstOrDefault();

                    Assert.NotNull(first);
                    Assert.Equal(first.Id, "tasks/1");
                    Assert.Equal(first.GiverId, 1);
                }
            }
        }
    }
}
