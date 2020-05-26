using GachiMail.Models;
using Microsoft.AspNetCore.Mvc;
using MailDatabase;
using System;

namespace GachiMail.Views.Register
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            if(!ViewData.ContainsKey("Mismatch"))
                ViewData["Mismatch"] = false;
            return View();
        }
        public IActionResult MailboxCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MBGo(string mailbox)
        {
            return RedirectToAction("Incoming", "Mailbox", new { box = mailbox});
        }

        [HttpPost]
        public IActionResult Go(User user, string passconf)
        {
            if (user.Password != passconf)
            {
                return PartialView("PasswordMismatch");
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
                        return PartialView("UserExists");
                }
                return RedirectToAction("MailboxCreate", "Mailbox");
            }
        }
    }
}