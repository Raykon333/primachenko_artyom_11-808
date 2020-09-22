using GachiMail.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System;

namespace GachiMail.Controllers
{
    public class AccountController : Controller
    {
        IDatabaseService db;

        public AccountController(IDatabaseService database)
        {
            db = database;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Mailbox");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginData, bool saveCookies)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Mailbox");
            if (!db.DoesUserExist(loginData.Login))
                ModelState.AddModelError("Login", "Пользователя не существует");
            else if (!db.PasswordCheck(loginData.Login, loginData.Password))
                ModelState.AddModelError("Password", "Пароль неверен");
            if (ModelState.IsValid)
            {
                await Authenticate(loginData.Login);
                return RedirectToAction("ProceedToMailbox", "Mailbox");
            }
            else
                return View(loginData);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Mailbox");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel regModel)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Mailbox");
            if (db.DoesUserExist(regModel.Login))
                ModelState.AddModelError("Login", "Пользователь уже существует");
            if (ModelState.IsValid)
            {
                db.AddUser(regModel.Login, regModel.Password);

                if (regModel.Login == "admin")
                {
                    db.ChangeRole("admin", "admin");
                }
                await Authenticate(regModel.Login);
                return RedirectToAction("NewMailbox", "Account");
            }
            return View(regModel);
        }

        private async Task Authenticate(string login)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, db.GetRole(login)),
                new Claim("Tier", db.GetTier(login).ToString())
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [HttpGet]
        public IActionResult NewMailbox()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewMailbox(NewMailboxModel model)
        {
            if (db.DoesMailboxExist(model.MailboxName))
                ModelState.AddModelError("MailboxName", "Ящик с таким названием уже существует.");
            if (ModelState.IsValid)
            {
                db.AddMailbox(User.Identity.Name, model.MailboxName);
                return RedirectToAction("ProceedToMailbox", "Mailbox");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!db.DoesUserExist(User.Identity.Name))
            {
                await Logout();
                return RedirectToAction("Index", "Home");
            }

            var login = User.Identity.Name;
            var currency = db.GetCurrency(login);
            var expiration = db.GetTierEndDate(login) - DateTime.Now;
            var tier = db.GetTier(login);
            if (tier > 0 && expiration < TimeSpan.Zero)
            {
                tier = 0;
                db.SetTier(login, 0, DateTime.Now);
            }

            var mailboxes = db.GetMailboxesByUser(login).ToArray();
            return View(new ProfileModel(login, tier, expiration, currency, mailboxes));
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddCurrency(int currency)
        {
            db.AddCurrency(User.Identity.Name, currency);
            return RedirectToAction("Profile");
        }

        [Authorize]
        public IActionResult UpgradeToTier1(ProfileModel model)
        {
            if (model.Currency >= 100)
            {
                db.SetTier(model.Login, 1, DateTime.Now + TimeSpan.FromSeconds(30));
                db.AddCurrency(model.Login, -100);
            }
            return RedirectToAction("Profile");
        }
    }
}