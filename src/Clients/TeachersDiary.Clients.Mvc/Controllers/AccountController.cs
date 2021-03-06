﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Expressions;

using Bytes2you.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using TeachersDiary.Clients.Mvc.Controllers.Abstracts;
using TeachersDiary.Clients.Mvc.ViewModels.Account;
using TeachersDiary.Data.Services.Contracts;

namespace TeachersDiary.Clients.Mvc.Controllers
{
    public class AccountController : BaseController
    {
        //TODO ISchoolService injection ??
        private readonly ISchoolService _schoolService;
        private readonly IAuthenticationService _authenticationService;

        public AccountController(
            ISchoolService schoolService,
            IAuthenticationService authenticationService)
        {
            Guard.WhenArgument(schoolService, nameof(schoolService)).IsNull().Throw();
            Guard.WhenArgument(authenticationService, nameof(authenticationService)).IsNull().Throw();

            _schoolService = schoolService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction<DashboardController>(x => x.Index());
            }
            
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authenticationService.LogIn(model.Email, model.Password);

            if (result == SignInStatus.Success)
            {
                return this.RedirectToAction<DashboardController>(x => x.Index());
            }

            ModelState.AddModelError(string.Empty, Resources.Resources.IncorrentEmailOrPasswordMessage);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return this.RedirectToAction<AccountController>(x => x.Login());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Register()
        {
            RegisterViewModel model = new RegisterViewModel()
            {
                Schools = await GetAllAvailableSchools()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Schools = await GetAllAvailableSchools();
                return View(model);
            }

            var result = await _authenticationService.CreateAccountAsync(model.Email, model.Password, model.SelectedSchool);

            if (result.Succeeded)
            {
                return this.RedirectToAction<DashboardController>(x => x.Index());
            }

            AddErrors(result);

            model.Schools = await GetAllAvailableSchools();
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authenticationService.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return this.RedirectToAction<DashboardController>(x => x.Index());
            }

            AddErrors(result);

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private async Task<IEnumerable<SelectListItem>> GetAllAvailableSchools()
        {
            var schoolNames = await _schoolService.GetAllSchoolNamesAsync();
            var schoolLists = schoolNames.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            schoolLists.Add(new SelectListItem()
            {
                Text = Resources.Resources.OtherLabel,
                Value = "-1"
            });

            return schoolLists;
        }
    }
}