using GachiMail.Models;
using Microsoft.AspNetCore.Mvc;
using MailDatabase;
using System;
<<<<<<< Updated upstream

=======
using Microsoft.AspNetCore.Http;
using MailDatabase.Exceptions;
>>>>>>> Stashed changes
namespace GachiMail.Views.Register
{
    public class RegisterController : Controller
    {
        public IActionResult Index(string message)
        {
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
            return RedirectToAction("ListMessages", "Mailbox", new { mtype = "Incoming"});
        }

        [HttpPost]
        public IActionResult Go(User user, string passconf)
        {
            if (user.Password != passconf)
            {
                return RedirectToAction
                    ("Index", 
                    "Register", 
                    new { message = "Passwords don't match"});
            }
            else
            {
<<<<<<< Updated upstream
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
=======
                if (ex is DatabaseException)
                    return RedirectToAction("Index", "Register", new { message = ex.Message });
>>>>>>> Stashed changes
            }
        }
    }
}