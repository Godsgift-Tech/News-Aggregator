using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Aggregator.EntityModel;
using News_Aggregator.Models;

namespace News_Aggregator.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = registerViewModel.Email,
                    Email = registerViewModel.Email
                };

                var applicationUserResult = await _userManager.CreateAsync(applicationUser, registerViewModel.Password);

                if (applicationUserResult.Succeeded)
                {
                    // Assign the user role
                    var roleapplicationUser = await _userManager.AddToRoleAsync(applicationUser, "user");

                    if (roleapplicationUser.Succeeded)
                    {
                        // Automatically sign the user in after registration
                        await _signInManager.SignInAsync(applicationUser, isPersistent: false);

                        // Redirect to the News page after successful registration
                        return RedirectToAction("Index", "News");
                    }
                }

                // Handle errors from registration
                foreach (var error in applicationUserResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If model state is invalid, return the view with validation messages
            return View(registerViewModel);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Redirect to the news page
                    return RedirectToAction("Index", "News"); // Assuming "Index" is your news page action
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(loginViewModel);
        }


        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            // Redirect to the "Index" action of the "News" controller after logging out
            return RedirectToAction("Index", "News");

        }

    }
}

