using AgileConfig.Server.Apisite.Auth;
using AgileConfig.Server.Apisite.UIExtension;
using AgileConfig.Server.Apisite.Websocket;
using AgileConfig.Server.Common;
using AgileConfig.Server.Data.Freesql;
using AgileConfig.Server.IService;
using AgileConfig.Server.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static IConfiguration Configuration { get; set; }

        public static IServiceCollection AddAgileServer(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Global.LoggerFactory = loggerFactory;
            Global.Config = configuration;
            Configuration = configuration;

            services.AddMemoryCache();
            services.AddAuthentication(AgileConfigAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<AgileConfigAuthOptions, AgileConfigAuthenticationHandler>(AgileConfigAuthenticationDefaults.AuthenticationScheme, o => { });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AgileConfig", policy =>
                {
                    policy.AuthenticationSchemes.Add(AgileConfigAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireClaim("name");
                });
            });
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddRazorRuntimeCompilation();
            services.AddFreeSqlDbContext();
            services.AddBusinessServices();
            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            return services;
        }

        public static IApplicationBuilder UseAgileServer(this IApplicationBuilder app)
        {
            app.UseMiddleware<ReactUIMiddleware>();

            app.UseCors(op =>
            {
                op.AllowAnyOrigin();
                op.AllowAnyMethod();
                op.AllowAnyHeader();
            });
            app.Map(Configuration.GetValue<string>("config_url") ?? "/home/index", a => a.Run(async context =>
            {
                context.Response.Redirect("/home/index");
                await Task.CompletedTask;
            }));
            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60),
                ReceiveBufferSize = 2 * 1024
            });
            app.UseMiddleware<WebsocketHandlerMiddleware>();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            var services = app.ApplicationServices;
            services.GetService<IRemoteServerNodeProxy>().TestEchoAsync();
            services.GetService<IEventRegister>().Init();

            return app;
        }
    }
}
