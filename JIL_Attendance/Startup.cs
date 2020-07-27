using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(JIL_Attendance.Startup))]
namespace JIL_Attendance
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
