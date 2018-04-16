using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinPlannerCodeFirst.Startup))]
namespace FinPlannerCodeFirst
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
