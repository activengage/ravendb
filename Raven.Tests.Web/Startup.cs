using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Raven35.Tests.Web.Startup))]
namespace Raven35.Tests.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
