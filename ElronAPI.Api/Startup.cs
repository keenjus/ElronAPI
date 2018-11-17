using ElronAPI.Api.Extensions;
using ElronAPI.Api.Hangfire;
using ElronAPI.Application.Behaviors;
using ElronAPI.Application.ElronAccount.Queries;
using FluentValidation.AspNetCore;
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
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Reflection;
using ElronAPI.Data.Models;

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

        private static bool EnableHangfire => !(bool.TryParse(Configuration["DisableHangfire"], out bool parsed) && parsed);

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

                if (EnableHangfire)
                {
                    services.AddHangfire(config =>
                        config.UsePostgreSqlStorage(Configuration.GetConnectionString("Peatus")));
                }
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
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehavior<,>));

            services.AddMediatR(typeof(ElronAccountQuery).GetTypeInfo().Assembly);

            services.AddCors(o => o.AddPolicy("AllowAllPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddSession();

            services.AddHttpClient("Elron", opts => { opts.BaseAddress = new Uri("https://pilet.elron.ee/"); });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ElronAccountQueryValidator>());
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (Environment.IsTest() == false)
            {
                GlobalDiagnosticsContext.Set("connectionString", Configuration.GetConnectionString("Elron"));
                loggerFactory.AddNLog().AddConsole().AddDebug();

                if (EnableHangfire)
                {
                    app.UseHangfireServer();
                    app.UseHangfireDashboard("/hangfire", new DashboardOptions()
                    {
                        Authorization = new[] { new TotpAuthorizationFilter(Configuration.GetValue<string>("TotpKey")) }
                    });

                    ConfigureHangfireJobs();
                }
            }

            app.UseSession();

            app.UseCors("AllowAllPolicy");
            app.UseMvc();
        }

        private static void ConfigureHangfireJobs()
        {
            RecurringJob.AddOrUpdate<GtfsImport>("gtfs-import", x => x.WorkAsync(), "0 1 */4 * *");
        }
    }
}