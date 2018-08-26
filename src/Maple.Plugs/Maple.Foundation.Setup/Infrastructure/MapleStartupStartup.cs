using Maple.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Maple.Foundation.Setup.Infrastructure.Extensions;


namespace Maple.Foundation.Setup.Infrastructure
{
    public class MapleStartupStartup : IMapleStartup
    {
        public int Order => 200;

        public void Configure(IApplicationBuilder application)
        {
            //检查数据库是否已经初始化，如果未初始化则跳转至初始化页面
            application.UseInstallUrl();
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {

        }
    }
}
