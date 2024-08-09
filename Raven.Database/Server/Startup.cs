using Owin;
using Raven35.Database.Config;

namespace Raven35.Database.Server
{
    internal class Startup
    {
        private readonly RavenDBOptions options;

        public Startup(InMemoryRavenConfiguration config, DocumentDatabase db = null)
        {
            options = new RavenDBOptions(config, db);
        }

        //Would prefer not to expose this
        public RavenDBOptions Options
        {
            get { return options; }
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseRavenDB(options);
        }
    }
}
