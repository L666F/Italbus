using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Italbus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Italbus.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if (roleManager.Roles.Count() == 0)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("CanManageUsers"));
                await roleManager.CreateAsync(new IdentityRole("CanEditEvents"));
                await roleManager.CreateAsync(new IdentityRole("CanEditHomepage"));
            }
            var admin = await userManager.FindByNameAsync("admin@italbus");
            await userManager.AddToRoleAsync(admin, "Admin");

            return View();
        }
    }
}