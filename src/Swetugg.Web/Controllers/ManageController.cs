//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Swetugg.Web.Models;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;

//namespace Swetugg.Web.Controllers
//{
//    [RequireHttps]
//    [Authorize]
//    public class ManageController : Microsoft.AspNetCore.Mvc.Controller
//    {
//        private ApplicationSignInManager _signInManager;
//        private ApplicationUserManager _userManager;

//        public ManageController()
//        {
//        }

//        /*public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
//        {
//            UserManager = userManager;
//            SignInManager = signInManager;
//        }*/

//        public ApplicationSignInManager SignInManager
//        {
//            get
//            {
//                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
//            }
//            private set 
//            { 
//                _signInManager = value; 
//            }
//        }

//        public ApplicationUserManager UserManager
//        {
//            get
//            {
//                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            }
//            private set
//            {
//                _userManager = value;
//            }
//        }

//        //
//        // GET: /Manage/Index
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(ManageMessageId? message)
//        {
//            ViewBag.StatusMessage =
//                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
//                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
//                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
//                : message == ManageMessageId.Error ? "An error has occurred."
//                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
//                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
//                : "";

//            var user = await UserManager.GetUserAsync(User);
//            var model = new IndexViewModel
//            {
//                HasPassword = HasPassword(),
//                PhoneNumber = await UserManager.GetPhoneNumberAsync(user),
//                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(user),
//                Logins = await UserManager.GetLoginsAsync(user),
//                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(user)
//            };
//            return View(model);
//        }

//        //
//        // POST: /Manage/RemoveLogin
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RemoveLogin(string loginProvider, string providerKey)
//        {
//            ManageMessageId? message;
//            var user = await UserManager.GetUserAsync(User);
//            var result = await UserManager.RemoveLoginAsync(user, loginProvider, providerKey);
//            if (result.Succeeded)
//            {
//                if (user != null)
//                {
//                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//                }
//                message = ManageMessageId.RemoveLoginSuccess;
//            }
//            else
//            {
//                message = ManageMessageId.Error;
//            }
//            return RedirectToAction("ManageLogins", new { Message = message });
//        }

//        //
//        // GET: /Manage/AddPhoneNumber
//        public Microsoft.AspNetCore.Mvc.ActionResult AddPhoneNumber()
//        {
//            return View();
//        }

//        //
//        // POST: /Manage/AddPhoneNumber
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }
//            // Generate the token and send it
//            var user = await UserManager.GetUserAsync(User);
//            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user, model.Number);
//            if (UserManager.SmsService != null)
//            {
//                var message = new IdentityMessage
//                {
//                    Destination = model.Number,
//                    Body = "Your security code is: " + code
//                };
//                await UserManager.SmsService.SendAsync(message);
//            }
//            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
//        }

//        //
//        // POST: /Manage/EnableTwoFactorAuthentication
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> EnableTwoFactorAuthentication()
//        {
//            var user = await UserManager.GetUserAsync(User);
//            await UserManager.SetTwoFactorEnabledAsync(user, true);
//            if (user != null)
//            {
//                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//            }
//            return RedirectToAction("Index", "Manage");
//        }

//        //
//        // POST: /Manage/DisableTwoFactorAuthentication
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> DisableTwoFactorAuthentication()
//        {
//            var user = await UserManager.GetUserAsync(User);
//            await UserManager.SetTwoFactorEnabledAsync(user, false);
//            if (user != null)
//            {
//                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//            }
//            return RedirectToAction("Index", "Manage");
//        }

//        //
//        // GET: /Manage/VerifyPhoneNumber
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> VerifyPhoneNumber(string phoneNumber)
//        {
//            var user = await UserManager.GetUserAsync(User);
//            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
//            // Send an SMS through the SMS provider to verify the phone number
//            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
//        }

//        //
//        // POST: /Manage/VerifyPhoneNumber
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }
//            var user = await UserManager.GetUserAsync(User);
//            var result = await UserManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
//            if (result.Succeeded)
//            {
//                if (user != null)
//                {
//                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//                }
//                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
//            }
//            // If we got this far, something failed, redisplay form
//            ModelState.AddModelError("", "Failed to verify phone");
//            return View(model);
//        }

//        //
//        // GET: /Manage/RemovePhoneNumber
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RemovePhoneNumber()
//        {
//            var user = await UserManager.GetUserAsync(User);
//            var result = await UserManager.SetPhoneNumberAsync(user, null);
//            if (!result.Succeeded)
//            {
//                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
//            }
//            if (user != null)
//            {
//                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//            }
//            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
//        }

//        //
//        // GET: /Manage/ChangePassword
//        public Microsoft.AspNetCore.Mvc.ActionResult ChangePassword()
//        {
//            return View();
//        }

//        //
//        // POST: /Manage/ChangePassword
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> ChangePassword(ChangePasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }
//            var user = await UserManager.GetUserAsync(User);
//            var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
//            if (result.Succeeded)
//            {
//                if (user != null)
//                {
//                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//                }
//                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
//            }
//            AddErrors(result);
//            return View(model);
//        }

//        //
//        // GET: /Manage/SetPassword
//        public Microsoft.AspNetCore.Mvc.ActionResult SetPassword()
//        {
//            return View();
//        }

//        //
//        // POST: /Manage/SetPassword
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> SetPassword(SetPasswordViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await UserManager.GetUserAsync(User);
//                var result = await UserManager.AddPasswordAsync(user, model.NewPassword);
//                if (result.Succeeded)
//                {
//                    if (user != null)
//                    {
//                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//                    }
//                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
//                }
//                AddErrors(result);
//            }

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }

//        //
//        // GET: /Manage/ManageLogins
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> ManageLogins(ManageMessageId? message)
//        {
//            ViewBag.StatusMessage =
//                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
//                : message == ManageMessageId.Error ? "An error has occurred."
//                : "";
//            var user = await UserManager.GetUserAsync(User);
//            if (user == null)
//            {
//                return View("Error");
//            }
//            var userLogins = await UserManager.GetLoginsAsync(user);
//            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
//            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
//            return View(new ManageLoginsViewModel
//            {
//                CurrentLogins = userLogins,
//                OtherLogins = otherLogins
//            });
//        }

//        //
//        // POST: /Manage/LinkLogin
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public Microsoft.AspNetCore.Mvc.ActionResult LinkLogin(string provider)
//        {
//            // Request a redirect to the external login provider to link a login for the current user
//            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), UserManager.GetUserId(User));
//        }

//        //
//        // GET: /Manage/LinkLoginCallback
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> LinkLoginCallback()
//        {
//            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, UserManager.GetUserId(User));
//            if (loginInfo == null)
//            {
//                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
//            }
//            var result = await UserManager.AddLoginAsync(UserManager.GetUserId(User), loginInfo.Login);
//            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && _userManager != null)
//            {
//                _userManager.Dispose();
//                _userManager = null;
//            }

//            base.Dispose(disposing);
//        }

//#region Helpers
//        // Used for XSRF protection when adding external logins
//        // private const string XsrfKey = "XsrfId";
//        private const string XsrfKey = "feBGOWgHWz4tWyxbmBBN";

//        private IAuthenticationManager AuthenticationManager
//        {
//            get
//            {
//                return HttpContext.GetOwinContext().Authentication;
//            }
//        }

//        private void AddErrors(IdentityResult result)
//        {
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error);
//            }
//        }

//        private bool HasPassword()
//        {
//            var user = UserManager.FindById(UserManager.GetUserId(User));
//            if (user != null)
//            {
//                return user.PasswordHash != null;
//            }
//            return false;
//        }

//        private bool HasPhoneNumber()
//        {
//            var user = UserManager.FindById(UserManager.GetUserId(User));
//            if (user != null)
//            {
//                return user.PhoneNumber != null;
//            }
//            return false;
//        }

//        public enum ManageMessageId
//        {
//            AddPhoneSuccess,
//            ChangePasswordSuccess,
//            SetTwoFactorSuccess,
//            SetPasswordSuccess,
//            RemoveLoginSuccess,
//            RemovePhoneSuccess,
//            Error
//        }

//#endregion
//    }
//}