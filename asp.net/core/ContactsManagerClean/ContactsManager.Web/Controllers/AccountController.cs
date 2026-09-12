using ContactsManager.Core.DTO;
using ContactsManager.Core.IdentityEntities;
using ContactsManager.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Web.Controllers
{
    [Route("[controller]/[action]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet] 
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return View(user);
            }

            ApplicationUser appUser = new ApplicationUser()
            {
                Email = user.Email,
                PhoneNumber = user.Phone,
                UserName = user.Email,
                Name    = user.Name,
            };

            IdentityResult result = await _userManager.CreateAsync(appUser, user.Password!);

            if (result.Succeeded)
            {
                if (await _roleManager.FindByNameAsync(user.UserRole.ToString()) is null)
                {
                    ApplicationRole appRole = new ApplicationRole()
                    {
                        Name = user.UserRole.ToString()
                    };
                    await _roleManager.CreateAsync(appRole);
                }
                if (user.UserRole == UserRole.Admin)
                {
                    
                    await _userManager.AddToRoleAsync(appUser, UserRole.Admin.ToString());
                }
                else
                {
                    await _userManager.AddToRoleAsync(appUser, UserRole.User.ToString());
                }
                // SIGN IN  
                await _signInManager.SignInAsync(appUser, isPersistent: false);
                return RedirectToAction(nameof(ContactsController.Index), "Contacts");
            } else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return View(user);
            }         
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Login(LoginDto user, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return View(user);
            }

            var result = await _signInManager.PasswordSignInAsync(user.Email!, user.Password!, 
                isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    return LocalRedirect(ReturnUrl); // security 
                return RedirectToAction(nameof(ContactsController.Index), "Contacts");
            }
            
            ModelState.AddModelError("Login", "Invalid email or password");
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(ContactsController.Index), "Contacts");
        }

        [HttpGet]
        public async Task<IActionResult> ValidateEmail(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) { return Json(true); }
            return Json(false); // invalid // email address already exists
        }
    }
}
