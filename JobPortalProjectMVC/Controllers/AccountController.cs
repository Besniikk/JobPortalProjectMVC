using JobPortalProjectMVC.Models;
using JobPortalProjectMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace JobPortalProjectMVC.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _roleManager = roleManager;
        }

        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect");
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> RegisterAsync()
        {
            var roles = await _roleManager.Roles
            .Where(r => (r.Name != "Admin" && r.Name != "User"))
            .Select(r => r.Name)
            .ToListAsync();

            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                Users users = new Users
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                };

                var result = await userManager.CreateAsync(users, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(users, new Claim("FullName", model.Name));

                    await userManager.AddToRoleAsync(users, model.SelectedRole);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors) 
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View();
                }
                
            }
            return View(model);
        }
        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Something is Wrong!");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                }
            }
            return View(model);
        }
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username });
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach(var error in result.Errors)
                        {
                            ModelState.AddModelError("",error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email is not found");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong. Try again");
                return View(model);

            }
            
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        
    }
}
