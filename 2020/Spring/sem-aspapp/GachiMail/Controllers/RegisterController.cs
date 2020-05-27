using GachiMail.Models;
using Microsoft.AspNetCore.Mvc;
using MailDatabase;
using System;
using Microsoft.AspNetCore.Http;
using GachiMail.Utilities.Encoder;
namespace GachiMail.Views.Register
{
    public class RegisterController : Controller
    {
        public IActionResult Index(int? code)
        {
            switch(code)
            {
                case 0:
                    ViewData["ErrorMessage"] = "User already Exists";
                    break;
                case 1:
                    ViewData["ErrorMessage"] = "Passwords don't match";
                    break;
                default:
                    break;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Go(User user, string passconf)
        {
            if (user.Password != passconf)
            {
                return RedirectToAction("Index", "Register", new { code = 1 });
            }
            try
            {
                DatabaseOperations.AddUser(user.Login, user.Password);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException && ex.Message == $"User {user.Login} already exists")
                    return RedirectToAction("Index", "Register", new { code = 0 });
            }
            HttpContext.Session.SetString("LI", "true");
            HttpContext.Session.SetString("User", user.Login);
            return RedirectToAction("MailboxCreate", "Mailbox", new { user = user.Login });
        }
    }
}