using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Postal;
using WorkOrders.Data;
using WorkOrders.Models;
using WorkOrders.Shared;

namespace WorkOrders.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly WorkOrdersContext _context;
        private readonly IEmailSenderEnhance _emailSender;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            WorkOrdersContext context,
            IEmailSenderEnhance emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(Login model)
        {
            if (!ModelState.IsValid || model == null)
            {
                ModelState.AddModelError("Error", "Wrong username or password");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: true);
            if(!result.Succeeded)
            {
                ModelState.AddModelError("Error", "Cannot log in. Please contact our admin");
                return View(model);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("Error", "Your account is locked. Please contact our admin for support");
                return View(model);
            }
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("Error", "Something happened, please contact our admin");
                return View(model);
            }
                return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            Register register = new Register();

            var userRoles = await _roleManager.Roles.ToListAsync();

            register.Roles = userRoles;
            return View(register);
        }
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            var userRoles = await _roleManager.Roles.ToListAsync();
            model.Roles = userRoles;

            if (!ModelState.IsValid || model == null)
            {
                ModelState.AddModelError("Error", "Please enter all of the required data");
                return View(model);
            }

            var user = new IdentityUser()
            {
                UserName = model.Email,
                Email = model.Email,
            };



            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Error", "Something went wrong. Please try again");
                return View(model);
            }
            await _userManager.AddToRoleAsync(user, model.Role);

            var emailData = new Email("ActivateAccount");
            emailData.RequestPath = Shared.Shared.PostalRequest(this.Request);

            emailData.ViewData["To"] = model.Email;
            emailData.ViewData["Username"] = model.Email;
            emailData.ViewData["Password"] = model.Password;

            await _emailSender.SendEmailAsync(emailData);


            var login = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: true);
            if (!login.Succeeded)
            {
                ModelState.AddModelError("Error", "Something went wrong");
                    return RedirectToAction("Login");
            }


            ModelState.AddModelError("Success", "Your account has been successfully created");
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.Username);

                if (user != null)
                {

                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //var token = "testtoken"; await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                        new { email = model.Username, token = token }, Request.Scheme);


                    var emailData = new Email("ForgotPassword");
                    emailData.RequestPath = Shared.Shared.PostalRequest(this.Request);
                    emailData.ViewData["Token"] = passwordResetLink;

                    await _emailSender.SendEmailAsync(emailData);



                    return View("ForgotPasswordConformation");
                }
                return View("ForgotPasswordConformation");
            }
            return View(model);
        }




    }
}
