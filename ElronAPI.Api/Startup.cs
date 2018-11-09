using ElronAPI.Api.Data;
using ElronAPI.Api.Extensions;
using ElronAPI.Api.Hangfire;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

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
            if (Environment.IsTest())
            {
                services.AddDbContext<ElronContext>(opt => opt.UseInMemoryDatabase("Elron"));
                services.AddDbContext<PeatusContext>(opt => opt.UseInMemoryDatabase("Peatus"));
            }
            else
            {
                services.AddDbContext<ElronContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Elron")));
                services.AddDbContext<PeatusContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Peatus")));

                services.AddHangfire(config => config.UsePostgreSqlStorage(Configuration.GetConnectionString("Peatus")));
            }


            services.AddMemoryCache();

            services.AddCors(o => o.AddPolicy("AllowAllPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddSession();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole().AddDebug();

            app.UseSession();

            if (!Environment.IsTest())
            {
                app.UseHangfireServer();
                app.UseHangfireDashboard("/hangfire", new DashboardOptions()
                {
                    Authorization = new[] { new TotpAuthorizationFilter(Configuration.GetValue<string>("TotpKey")) }
                });
            }

            app.UseCors("AllowAllPolicy");
            app.UseMvc();
        }
    }
}