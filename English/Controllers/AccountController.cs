using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using english.DbFolder;
using english.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using MimeKit;

namespace Bilim.Controllers
{
    public class AccountController : Controller
    {
        AppDb db;
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;


        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, AppDb _db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            db = _db;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { UserName = model.PhoneNumber, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Profil", "Account");
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


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("UserList", "Admin");
                    //return RedirectToAction("UserList", "Admin", new { page = 1 });
                }
                else
                {
                    return RedirectToAction("Profil", "Account");
                }
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.PhoneNumber, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.PhoneNumber);
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Profil", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Құпиясөз және(немесе) логин қате!");
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



        [Authorize]
        public async Task<IActionResult> Profil()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("UserList", "Admin");
                    //return RedirectToAction("UserList", "Admin", new { page = 1 });
                }
                else
                {
                    var uk = await db.UserKurs.Where(x => x.UserId == user.Id).ToListAsync();
                    ViewBag.kurses = await db.Kurs.ToListAsync();
                    return View(uk);
                }
            }
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Video(int id, int videoId = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("UserList", "Admin");
                    //return RedirectToAction("UserList", "Admin", new { page = 1 });
                }
                else
                {
                    var kurs = await db.Kurs.FirstOrDefaultAsync(x => x.Id == id);
                    KursVideo video;
                    List<KursVideo> otherVideos;
                    if (videoId == 0)
                    {
                        video = await db.KursVideos.FirstOrDefaultAsync(x => x.KursId == kurs.Id);
                        otherVideos = await db.KursVideos.Where(x => x.Id != video.Id && x.KursId == id).Include(p => p.Kurs).ToListAsync();
                    }
                    else
                    {
                        video = await db.KursVideos.FirstOrDefaultAsync(x => x.Id == videoId && x.KursId == kurs.Id);
                        otherVideos = await db.KursVideos.Where(x => x.Id != videoId && x.KursId == id).Include(p => p.Kurs).ToListAsync();
                    }
                    ViewBag.OtherVideos = otherVideos;

                    return View(video);
                }
            }
            return RedirectToAction("Login");
        }



        public IActionResult AllKurs()
        {
            return View(db.Kurs.ToList());
        }

        public IActionResult News()
        {
            var list = db.Kurs.Where(p => p.CreatDate >= DateTime.Now).ToList();

            return View(list);
        }



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

            
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);
        //        if (user == null)
        //        {
        //            // пользователь с данным email может отсутствовать в бд
        //            // тем не менее мы выводим стандартное сообщение, чтобы скрыть 
        //            // наличие или отсутствие пользователя в бд
        //            return View("Login");
        //        }

        //        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        var callbackUrl = Url.Action("ResetPassword", "Account",
        //            new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);

        //        EmailService emailService = new EmailService();

        //        try
        //        {
        //            await emailService.SendEmailAsync(model.Email, "Reset Password",
        //                $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
        //            return View("ForgotPasswordConfirm");
        //        }
        //        catch (Exception err)
        //        {
        //            return Content("err: " + err);
        //        }

        //    }
        //    return View(model);
        //}
        //[HttpGet]
        //public IActionResult ForgotPasswordConfirm()
        //{
        //    return View();
        //}


        //[HttpGet]
        //public IActionResult ResetPassword(string code = null)
        //{
        //    return code == null ? View("ForgotPassword") : View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return View("ResetPasswordConfirm");
        //    }
        //    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return View("ResetPasswordConfirm");
        //    }
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }
        //    return View(model);
        //}
        //[HttpGet]
        //public IActionResult ResetPasswordConfirm()
        //{
        //    return View();
        //}
    }
}