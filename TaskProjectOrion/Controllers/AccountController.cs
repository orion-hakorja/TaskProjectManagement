using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskProjectOrion.Models;
using TaskProjectOrion.ViewModels;

namespace TaskProjectOrion.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    
    public AccountController(UserManager<ApplicationUser> userManagerr, SignInManager<ApplicationUser> signInManagerr)
    {
    this.userManager = userManagerr;
    this.signInManager = signInManagerr;
    }

    public IActionResult Register()
    {
        return View();
    }
    
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid registration attempt");
            return View(model);
        }

        var user = new ApplicationUser
        {
        UserName = model.UserName,
        Email =  model.Email
        };
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid registration attempt");
            return View(model);
        }

        if (model.UserName == "admin" || model.UserName == "Admin" || model.UserName == "Orion")
        {
            var test = await userManager.AddToRoleAsync(user, "Admin");
            if (!test.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid registration attempt");
                return View(model);
            }
        }
        else
        {
             var test = await userManager.AddToRoleAsync(user, "User");
             if (!test.Succeeded)
             {
                 ModelState.AddModelError(string.Empty, "Invalid registration attempt");
                 return View(model);
             }
        }    
        await signInManager.PasswordSignInAsync(user,model.Password,false,false);
        TempData["Success"] = "Your successfully registered";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "There is no user with this email address");
            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(user, model.Password, false,false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Could not log in");
            return View(model);
        }
        TempData["Success"] = "Your successfully logged in!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        TempData["Success"] = "You logged out";
        return RedirectToAction("Login", "Account");
    }
}