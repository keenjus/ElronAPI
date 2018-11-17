using ElronAPI.Api.Data;
using ElronAPI.Api.Extensions;
using ElronAPI.Api.Hangfire;
using ElronAPI.Application.ElronAccount.Queries;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using ElronAPI.Application.Behaviors;
using FluentValidation.AspNetCore;

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
            if (Environment.IsTest() == false)
            {
                services.AddDbContext<ElronContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Elron")));
                services.AddDbContext<PeatusContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Peatus")));

                services.AddHangfire(config =>
                    config.UsePostgreSqlStorage(Configuration.GetConnectionString("Peatus")));
            }
            else
            {
                services.AddDbContext<ElronContext>(opt => opt.UseInMemoryDatabase("Elron"));
                services.AddDbContext<PeatusContext>(opt => opt.UseInMemoryDatabase("Peatus"));
            }

            services.AddMemoryCache();
            services.AddHttpClient();

            services.AddScoped<ServiceFactory>(p => p.GetService);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            services.AddMediatR(typeof(ElronAccountQuery).GetTypeInfo().Assembly);

            services.AddCors(o => o.AddPolicy("AllowAllPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddSession();

            services.AddHttpClient();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ElronAccountQueryValidator>());
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole().AddDebug();

            app.UseSession();

            if (Environment.IsTest() == false)
            {
                app.UseHangfireServer();
                app.UseHangfireDashboard("/hangfire", new DashboardOptions()
                {
                    Authorization = new[] { new TotpAuthorizationFilter(Configuration.GetValue<string>("TotpKey")) }
                });

                ConfigureHangfireJobs();
            }

            app.UseCors("AllowAllPolicy");
            app.UseMvc();
        }

        private static void ConfigureHangfireJobs()
        {
            RecurringJob.AddOrUpdate<GtfsImport>("gtfs-import", x => x.WorkAsync(), "0 1 */4 * *");
        }
    }
}