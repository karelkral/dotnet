using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using brechtbaekelandt.ldap.Services;
using brechtbaekelandt.ldap.Identity;

using brechtbaekelandt.ldap.ViewModels;

namespace brechtbaekelandt.ldap.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly LdapUserManager _userManager;
        private readonly LdapSignInManager _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccountController(
            LdapUserManager userManager,
            LdapSignInManager signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailSender = emailSender;
            this._logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Signin(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ViewData["ReturnUrl"] = returnUrl;

            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin(SigninViewModel model, string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;

            if (this.ModelState.IsValid)
            {
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return this.RedirectToLocal(returnUrl);
                    }

                    // I added the exclamation mark to make it more dramatic
                    this.TempData["ErrorMessage"] = "The username and/or password are incorrect!";

                    return this.View(model);
                }
                catch (Exception)
                {
                    this.TempData["ErrorMessage"] = "Something bad happened while logging in...";

                    return this.View(model);
                }
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            else
            {
                return this.RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
