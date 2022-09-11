using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swetugg.Web.Models;
using System;
using System.Linq;

namespace Swetugg.Web
{
	public static class InitDB
	{
		public static void Init(IConfiguration configuration, IServiceProvider services)
		{
			var context = services.GetRequiredService<ApplicationDbContext>();
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
					if (context.Database.GetPendingMigrations().Any())
					{
						context.Database.EnsureDeleted();
					}
					context.Database.Migrate();
					break;
				case "MigrateDatabaseToLatestVersion":
					context.Database.Migrate();
					break;
				default:
					context.Database.Migrate();
					break;
			}

			// TODO: Move the seed into migrations once we've verified that the .net framework => .net 6 upgrade is functional

			var rolesToAdd = new string[]
			{
				"ConferenceManager",
				"VipSpeaker",
				"User"
			}.Where(roleName => !context.Roles.Any(role => role.Name.Equals(roleName)));

			if (rolesToAdd.Any())
			{
				var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
				foreach(var roleName in rolesToAdd)
				{
					roleManager.CreateAsync(new IdentityRole { Name = roleName });
				}
			}

			if (!context.Users.Any())
			{
				// If no users have been created yet, insert a default Admin account. 
				var manager = services.GetRequiredService<UserManager<ApplicationUser>>();
				var user = new ApplicationUser { UserName = "info@swetugg.se", Email = "info@swetugg.se" };

				manager.CreateAsync(user, "ChangeMe.123").Wait();
				manager.AddToRolesAsync(user, new string[] { "Administrator", "ConferenceManager", "User" }).Wait();
			}
		}
	}
}
