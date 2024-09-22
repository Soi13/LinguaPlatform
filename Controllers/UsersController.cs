using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinguaPlatform.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace LinguaPlatform.Controllers
{
    public class UsersController : Controller
    {
        UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //Function for getting response form Html link like a JSON object
        public string GetResponseText(string url, string postData)
        {
            string responseText = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = postData.Length;
            using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(postData);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }
            return responseText;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index() => View(_userManager.Users.ToList());

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUser model)
        {
            if (ModelState.IsValid)
            {
                //Creating user for whiteboard and getting token of user. It will be necessary for opening whiteboard without signing up to Limnu
                //dynamic LimnuWhiteBoardUser = JObject.Parse(GetResponseText("https://api.limnu.com/v1/userCreate", "{\"apiKey\":\"00000000000000000000000\", \"displayName\":\" " + model.Email + " \"}"));
                dynamic LimnuWhiteBoardUser = JObject.Parse(GetResponseText("https://api.limnu.com/v1/userCreate", "{\"apiKey\":\"0000000000000000000000000\", \"displayName\":\"Group user\"}"));
                string LimnuToken = LimnuWhiteBoardUser.token;
                string LimnuUserId = LimnuWhiteBoardUser.userId;
                string LimnuSecret = LimnuWhiteBoardUser.secret;


                User user = new User { Email = model.Email, UserName = model.Email, LimnuToken = LimnuToken, IsActive = true, LimnuUserId = LimnuUserId, LimnuSecret = LimnuSecret };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {                   
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUser model = new EditUser { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditUser model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePassword model = new ChangePassword { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(model);
        }



    }
}
