using AgileConfig.Server.Common;
using AgileConfig.Server.IService;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System.IO;

namespace AgileConfig.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            Global.Config =
                 builder
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables()
                .Build();
#else
            Global.Config = builder.AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
#endif
            var host = CreateHostBuilder(args)
                .Build();

            var sp = host.Services;
            sp.GetService<IRemoteServerNodeProxy>().TestEchoAsync();
            sp.GetService<IEventRegister>().Init();

            host.Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseNLog()
                .UseStartup<AgileConfig.Server.Apisite.Startup>();
    }
}
