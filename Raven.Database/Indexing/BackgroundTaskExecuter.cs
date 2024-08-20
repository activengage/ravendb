using Raven35.Abstractions.Extensions;

namespace Raven35.Database.Indexing
{
    public class BackgroundTaskExecuter
    {
        public static IBackgroundTaskExecuter Instance = new DefaultBackgroundTaskExecuter();
    }
}
