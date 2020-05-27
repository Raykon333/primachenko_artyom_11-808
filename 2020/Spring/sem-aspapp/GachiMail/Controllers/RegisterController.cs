using GachiMail.Models;
using Microsoft.AspNetCore.Mvc;
using MailDatabase;
using System;
using Microsoft.AspNetCore.Http;
using MailDatabase.Exceptions;
using System.Linq;
namespace GachiMail.Views.Register
{
    public class RegisterController : Controller
    {
        public IActionResult Index(int? code)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Go(User user, string passconf)
        {
            if (user.Password != passconf)
            {
                HttpContext.Session.SetString("ErrorMessage", "Passwords don't match");
                return RedirectToAction("Index", "Register");
            }
            try
            {
                DatabaseOperations.AddUser(user.Login, user.Password);
            }
            catch (Exception ex)
            {
                if (ex is DatabaseException)
                {
                    HttpContext.Session.SetString("ErrorMessage", ex.Message);
                    return RedirectToAction("Index", "Register");
                }     
            }
            if (HttpContext.Session.Keys.Contains("ErrorMessage"))
                HttpContext.Session.Remove("ErrorMessage");
            HttpContext.Session.SetString("LI", "true");
            HttpContext.Session.SetString("User", user.Login);
            return RedirectToAction("MailboxCreate", "Mailbox", new { user = user.Login });
        }
    }
}