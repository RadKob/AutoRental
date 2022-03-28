using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AutoRental.Startup))]
namespace AutoRental
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
