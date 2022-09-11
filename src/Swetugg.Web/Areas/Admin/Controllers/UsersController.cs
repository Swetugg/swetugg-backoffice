using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Area("Admin")]
    [Route("users")]
    public class UsersController : Microsoft.AspNetCore.Mvc.Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public UsersController()
        {
        }

        /*public UsersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }*/

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        [Route("")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index()
        {
            var users = await UserManager.Users.ToListAsync();
            var roles = await RoleManager.Roles.ToListAsync();
            
            ViewBag.Roles = roles;
            
            return View(users);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("set-roles")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> SetRoles(string userId, List<string> roles)
        {
            var currentRoles = await UserManager.GetRolesAsync(userId);
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
                        await UserManager.RemoveFromRoleAsync(userId, currentRole);

                    }
                }
            }
            var result = await UserManager.AddToRolesAsync(userId, roles.ToArray());

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("delete")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(string userId)
        {
            var user = await UserManager.Users.FirstAsync(u => u.Id == userId);
            var result = await UserManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }
    }
}