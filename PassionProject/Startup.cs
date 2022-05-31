using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PassionProject.Startup))]
namespace PassionProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
