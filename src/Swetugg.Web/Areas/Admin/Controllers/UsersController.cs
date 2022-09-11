using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Area("Admin")]
    [Route("users")]
    public class UsersController : Microsoft.AspNetCore.Mvc.Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationUser> _roleManager;

        public UsersController()
        {
        }

		public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationUser> roleManager)
		{
			_userManager = userManager;
            _roleManager = roleManager;

		}

        [Route("")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            
            ViewBag.Roles = roles;
            
            return View(users);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("set-roles")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> SetRoles(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var currentRoles = await _userManager.GetRolesAsync(user);
            roles = roles ?? new List<string>();
            if (currentRoles != null)
            {
                foreach (var currentRole in currentRoles)
                {
                    if (roles.Contains(currentRole))
                    {
                        // User is already in role, no need to add again
                        roles.Remove(currentRole);
                    }
                    else
                    {
                        // User should no longer be in this role
                        await _userManager.RemoveFromRoleAsync(user, currentRole);

                    }
                }
            }
            var result = await _userManager.AddToRolesAsync(user, roles.ToArray());

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("delete")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);
            var result = await _userManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }
    }
}