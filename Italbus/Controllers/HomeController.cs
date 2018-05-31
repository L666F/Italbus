using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Italbus.Models;
using Microsoft.AspNetCore.Identity;
using Italbus.Models.ViewModels;

namespace Italbus.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("/login",Name = "Login")]
        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Admin");
            else
                return View(new LoginVM { Message = "" });
        }
        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            vm.Message = null;
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(vm.Username);
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user, vm.Password,false,false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        vm.Message = "Wrong Username or Password";
                        return View(vm);
                    }
                }
                else
                {
                    vm.Message = "Wrong Username or Password";
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }
        [Route("/Home/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
