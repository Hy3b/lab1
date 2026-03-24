using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Lab01_WebMVC.Models;
using Lab01_WebMVC.Models.ViewModels;
using Lab01_WebMVC.Services;

namespace Lab01_WebMVC.Controllers;

[Route("[controller]/[action]")]
public class AccountController : BaseController {
    private readonly UserManager<ApplicationUser> _um;
    private readonly SignInManager<ApplicationUser> _sm;

    public AccountController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm, ICartService cart) : base(cart) {
        _um = um;
        _sm = sm;
    }

    [HttpGet] public IActionResult Login(string? returnUrl)
        => View(new LoginVM { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var r = await _sm.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: true);
        if (r.IsLockedOut)
        { 
            ModelState.AddModelError("","Tài khoản bị khóa tạm thời"); 
            return View(vm); 
        }
        if (!r.Succeeded)
        { 
            ModelState.AddModelError("","Email hoặc mật khẩu không đúng"); 
            return View(vm); 
        }
        return LocalRedirect(vm.ReturnUrl ?? "/");
    }

    [HttpGet] public IActionResult Register() => View(new RegisterVM());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var user = new ApplicationUser { UserName=vm.Email, Email=vm.Email, FullName=vm.FullName };
        var r = await _um.CreateAsync(user, vm.Password);
        if (!r.Succeeded)
        { 
            foreach(var e in r.Errors) ModelState.AddModelError("",e.Description);
            return View(vm); 
        }
        await _um.AddToRoleAsync(user, "Customer");
        await _sm.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index","Home");
    }

    [HttpPost, Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    { 
        await _sm.SignOutAsync(); 
        return RedirectToAction("Index","Home"); 
    }
}
