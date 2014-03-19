using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestWebEssentialsBug.Startup))]
namespace TestWebEssentialsBug
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
