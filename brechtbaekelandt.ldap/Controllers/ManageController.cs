using System;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using brechtbaekelandt.ldap.Extensions;
using brechtbaekelandt.ldap.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using brechtbaekelandt.ldap.Services;
using brechtbaekelandt.ldap.Identity.Models;
using brechtbaekelandt.ldap.ViewModels;

namespace brechtbaekelandt.ldap.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly LdapUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;

        public IActionResult Index()
        {
            var vm = new ManageViewModel
            {
                Users = this._userManager.Users.ToCollection()
            };

            return this.View(vm);
        }

        public ManageController(
          LdapUserManager userManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm]LdapUser newUser)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    var result = await this._userManager.CreateAsync(newUser, newUser.Password);

                    if (!result.Succeeded)
                    {
                        this.TempData["CreateErrorMessage"] = result.Errors.Any() ? result.Errors.First().Description : "There was a problem creating the user.";
                    }
                    else
                    {
                        this.TempData["CreateSucceededMessage"] = "The user was sucessfully created.";
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            var vm = new ManageViewModel
            {
                Users = this._userManager.Users.ToCollection(),
                NewUser = newUser
            };

            return this.View("Index", vm); ;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromQuery]string distinguishedName)
        {
            if (this.ModelState.IsValid)
            {
                try
                {


                    var result = await this._userManager.DeleteUserAsync(distinguishedName);

                    if (!result.Succeeded)
                    {
                        this.TempData["DleteErrorMessage"] = result.Errors.Any() ? result.Errors.First().Description : "There was a problem deleting the user.";
                    }
                    else
                    {
                        this.TempData["DeleteSucceededMessage"] = "The user was sucessfully deleted.";
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            var vm = new ManageViewModel
            {
                Users = this._userManager.Users.ToCollection()
            };

            return this.View("Index", vm); ;
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}
