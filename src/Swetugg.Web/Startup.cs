// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swetugg.Web.Models;

namespace Swetugg.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddIdentityCore<ApplicationUser>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddMemoryCache()
                .AddApplicationInsightsTelemetry()
                .AddControllersWithViews(ConfigureMvcOptions)
                // Newtonsoft.Json is added for compatibility reasons
                // The recommended approach is to use System.Text.Json for serialization
                // Visit the following link for more guidance about moving away from Newtonsoft.Json to System.Text.Json
                // https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to
                .AddNewtonsoftJson(options =>
                {
                    options.UseMemberCasing();
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin",
                    areaName: "Admin",
                    pattern: "admin/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Cfp",
                    areaName: "Cfp",
                    pattern: "cfp/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Sthlm2023",
                    areaName: "Sthlm2023",
                    pattern: "sthlm-2023/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Gbg2022",
                    areaName: "Gbg2022",
                    pattern: "gbg-2022/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Sthlm2020",
                    areaName: "Sthlm2020",
                    pattern: "sthlm-2020/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Sthlm2019",
                    areaName: "Swetugg2019",
                    pattern: "swetugg-2019/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Sthlm2018",
                    areaName: "Swetugg2018",
                    pattern: "swetugg-2018/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Sthlm2017",
                    areaName: "Swetugg2017",
                    pattern: "swetugg-2017/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Sthlm2016",
                    areaName: "Swetugg2016",
                    pattern: "swetugg-2016/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Sthlm2015",
                    areaName: "Swetugg2015",
                    pattern: "swetugg-2015/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            InitDB(Configuration, app.ApplicationServices.GetRequiredService<ApplicationDbContext>());
        }

        private void ConfigureMvcOptions(MvcOptions mvcOptions)
        { 
        }

        private void InitDB(IConfiguration configuration, ApplicationDbContext context)
		{
            switch (configuration["Database_Initialize_Strategy"])
            {
                case "CreateDatabaseIfNotExists":
                    context.Database.Migrate();
                    break;
                case "DropCreateDatabaseAlways":
                    context.Database.EnsureDeleted();
                    context.Database.Migrate();
                    break;
                case "DropCreateDatabaseIfModelChanges":
                    if(context.Database.GetPendingMigrations().Any())
					{
                        context.Database.EnsureDeleted();
                    }
                    context.Database.Migrate();
                    break;
                case "MigrateDatabaseToLatestVersion":
                    context.Database.Migrate();
                    break;
                default:
                    context.Database.EnsureCreated();
                    break;
            }
        }
    }
}
