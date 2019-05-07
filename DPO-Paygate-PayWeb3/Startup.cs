using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DPO_Paygate_PayWeb3.Startup))]
namespace DPO_Paygate_PayWeb3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
