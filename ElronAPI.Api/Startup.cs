using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ElronAPI.Api.Data;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using OtpNet;

namespace ElronAPI.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private static IConfiguration Configuration { get; set; }
        private static IHostingEnvironment Environment { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsEnvironment("Test"))
            {
                services.AddDbContext<ElronContext>(opt => opt.UseInMemoryDatabase("Elron"));
                services.AddDbContext<PeatusContext>(opt => opt.UseInMemoryDatabase("Peatus"));
            }
            else
            {
                services.AddDbContext<ElronContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Elron")));
                services.AddDbContext<PeatusContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Peatus")));
            }

            services.AddHangfire(config => config.UsePostgreSqlStorage(Configuration.GetConnectionString("Peatus")));

            services.AddMemoryCache();

            services.AddCors(o => o.AddPolicy("AllowAllPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole().AddDebug();

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            app.UseCors("AllowAllPolicy");
            app.UseMvc();
        }
    }

    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private const string Key = "KDK5M4JDLELMFMV3";
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (httpContext.Request.Cookies.TryGetValue("auth", out string value))
            {
                return value == Key;
            }

            if (httpContext.Request.Query.TryGetValue("auth", out var key) && key == Key)
            {
                httpContext.Response.Cookies.Append("auth", Key);
                return true;
            }

            return false;
        }

        public static bool IsLocal(HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }
                else
                {
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);
                }
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }
    }
}