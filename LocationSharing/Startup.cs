using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LocationSharing.Startup))]
namespace LocationSharing
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
