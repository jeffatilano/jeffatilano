
using JIL_Attendance.Helpers;
using JIL_Attendance.Models;
using JIL_Attendance.ViewModel;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JIL_Attendance.Controllers
{
    [Authorize]
    public class UserAdminController : Controller
    {
        public UserAdminController()
        {
        }

        public UserAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        // GET: UserAdmin
        public ActionResult Index()
        {
            var userList = UserManager.Users.OrderByDescending(x => x.Email).ToList();
            return View(userList);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            ViewData["Church"] = await DdlHelpers.ChurchList();
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            EditUserViewModel test = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                // Include the Addresss info:
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            };
            return View(test);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditUserViewModel editUser, int Church, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.Email = editUser.Email;
                user.ChurchID = Church;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }
        // GET: /Users/Create
        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser
        //        {
        //            UserName = userViewModel.UserName,
        //            Email = userViewModel.Email,
        //            // Add User Info: - added by Romel 11/19/2014
        //            Firstname = userViewModel.Firstname,
        //            Lastname = userViewModel.Lastname,
        //            PhoneNumber = userViewModel.PhoneNumber,
        //            ChurchID = userViewModel.ChurchID
        //        };
    }
}