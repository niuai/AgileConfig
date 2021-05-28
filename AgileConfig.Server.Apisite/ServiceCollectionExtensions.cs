using AgileConfig.Server.Apisite;
using AgileConfig.Server.Apisite.UIExtension;
using AgileConfig.Server.Apisite.Websocket;
using AgileConfig.Server.Common;
using AgileConfig.Server.Data.Freesql;
using AgileConfig.Server.IService;
using AgileConfig.Server.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAgileServer(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Global.LoggerFactory = loggerFactory;
            Global.Config = configuration;

            services.AddMemoryCache();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = JwtSetting.Instance.Issuer,
                        ValidAudience = JwtSetting.Instance.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSetting.Instance.SecurityKey)),
                    };
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
