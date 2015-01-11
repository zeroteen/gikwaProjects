using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin;
using Owin;
using Grikwa.Models;

[assembly: OwinStartupAttribute(typeof(Grikwa.Startup))]
namespace Grikwa
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
            var idProvider = new PrincipalUserIdProvider();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);

            // restore all rooms which were created
            ChatRooms.RestoreRooms();
            //Session
        }
    }
}
