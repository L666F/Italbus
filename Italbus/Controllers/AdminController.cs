using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Italbus.Data;
using Italbus.Models;
using Italbus.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Italbus.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AdminController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            this.dbContext = dbContext;
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

        public ActionResult EditHomepage()
        {
            var sections = dbContext.Sections.ToList();
            return View(sections);
        }

        public ActionResult EditSection(int? id)
        {
            if (id == null)
                return RedirectToAction("EditHomepage", "Admin");

            var section = dbContext.Sections.SingleOrDefault(m => m.ID == id);
            return View(section);
        }

        [HttpPost]
        public ActionResult EditSection(Section section)
        {
            if (ModelState.IsValid)
            {
                Section prev = dbContext.Sections.Single(m => m.ID == section.ID);
                prev.Text = section.Text;
                prev.Title = section.Title;
                dbContext.SaveChanges();
                return RedirectToAction("EditHomepage", "Admin");
            }
            else
            {
                return View(section);
            }
        }

        public ActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToRoute(routeName:"Login");
        }

        public ActionResult ManageUsers()
        {
            var users = userManager.Users.Where(m => m.UserName!="admin@italbus" && m.UserName!=User.Identity.Name).ToList();
            return View(users);
        }

        public async Task<ActionResult> DeleteUser(string id)
        {
            if (id != null)
                await userManager.DeleteAsync(await userManager.FindByIdAsync(id));
            return RedirectToAction("ManageUsers", "Admin");
        }

        public async Task<ActionResult> EditRole(string id, string role)
        {
            var user = await userManager.FindByIdAsync(id);
            if(role == "homepage")
            {
                if (await userManager.IsInRoleAsync(user, "CanEditHomepage"))
                {
                    await userManager.RemoveFromRoleAsync(user, "CanEditHomepage");
                }
                else
                {
                    await userManager.AddToRoleAsync(user, "CanEditHomepage");
                }
                return RedirectToAction("ManageUsers", "Admin");
            }
            if (role == "events")
            {
                if (await userManager.IsInRoleAsync(user, "CanEditEvents"))
                {
                    await userManager.RemoveFromRoleAsync(user, "CanEditEvents");
                }
                else
                {
                    await userManager.AddToRoleAsync(user, "CanEditEvents");
                }
                return RedirectToAction("ManageUsers", "Admin");
            }
            if (role == "users")
            {
                if (await userManager.IsInRoleAsync(user, "CanManageUsers"))
                {
                    await userManager.RemoveFromRoleAsync(user, "CanManageUsers");
                }
                else
                {
                    await userManager.AddToRoleAsync(user, "CanManageUsers");
                }
                return RedirectToAction("ManageUsers", "Admin");
            }
            return RedirectToAction("ManageUsers", "Admin");
        }
    }
}