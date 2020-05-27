using GachiMail.Models;
using Microsoft.AspNetCore.Mvc;
using MailDatabase;
using System;

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
        public IActionResult MailboxCreate(string user, int? code)
        {
            if (code != null)
                ViewData["ErrorMessage"] = "Mailbox already exists.";
            ViewData["User"] = user;
            return View();
        }

        [HttpPost]
        public IActionResult MBGo(string mailbox, string user)
        {
            try
            {
                DatabaseOperations.AddMailbox(user, mailbox);
            }
            catch(Exception ex)
            {
                if (ex is ArgumentException)
                    return RedirectToAction("MailboxCreate", "Register", new { user = user, code = 0 });
            }
            return RedirectToAction("Incoming", "Mailbox", new { box = mailbox});
        }

        [HttpPost]
        public IActionResult Go(User user, string passconf)
        {
            if (user.Password != passconf)
            {
                return RedirectToAction("Index", "Register", new { code = 1 });
            }
            else
            {
                try
                {
                    DatabaseOperations.AddUser(user.Login, user.Password);
                }
                catch(Exception ex)
                {
                    if (ex is ArgumentException && ex.Message == $"User {user.Login} already exists")
                        return RedirectToAction("Index", "Register", new { code = 0 });
                }
                return RedirectToAction("MailboxCreate", "Register", new { user = user.Login });
            }
        }
    }
}