using Microsoft.Owin;
using Owin;
[assembly: OwinStartupAttribute(typeof(Forum2.Startup))]
namespace Forum2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
