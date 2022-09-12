using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swetugg.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

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

Swetugg.Web.InitDB.Init(builder.Configuration, app.Services);

app.Run();