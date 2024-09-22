using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinguaPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace LinguaPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        ConnectDb cdb = new ConnectDb();

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new Login { ReturnUrl = returnUrl });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([FromBody] Login model)
        {       
            if (ModelState.IsValid)
            {
                if (!cdb.CheckUserStatus(model.Email))
                {
                    return new JsonResult(new { text = "Your account is blocked." });
                }
                else
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,  Convert.ToBoolean(model.RememberMe), true);
                    if (result.Succeeded)
                    {
                        // проверяем, принадлежит ли URL приложению
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return new JsonResult(new { text = true });
                        }
                    }
                    else if (result.IsLockedOut)
                    {
                        return new JsonResult(new { text = "Your account is locked out. Kindly wait for 10 minutes and try again." });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect Login or Password.");
                    }

                    string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return new JsonResult(new { text = error_messages });
                }
            }
            else
            {
                //string error_messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                //return new JsonResult(new { text = error_messages });
                return new JsonResult(new { text = "Not all data valid!" });
            }
            //return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // пользователь с данным email может отсутствовать в бд
                    // тем не менее мы выводим стандартное сообщение, чтобы скрыть 
                    // наличие или отсутствие пользователя в бд
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);                
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Reset Password", $"To reset your password follow the link: <a href='{callbackUrl}'>link</a>");

               return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Logout()
        {
            // удаляем аутентификационные куки
             _signInManager.SignOutAsync();
            //return RedirectToAction("Login", "Account");
            return new JsonResult(new { text = true });
        }
    }
}
