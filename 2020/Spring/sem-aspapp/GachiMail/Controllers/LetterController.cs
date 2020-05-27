using System;
using GachiMail.Utilities;
using Microsoft.AspNetCore.Mvc;
using GachiMail.Models;
using MailDatabase;
namespace GachiMail.Controllers
{
    public class LetterController : MailAccountController
    {
        public IActionResult Index(int id)
        {
            ViewData["Mail"] = new Letter(id);
            return View();
        }
<<<<<<< Updated upstream
=======
        public IActionResult WriteLetter()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Send(string Title,
                                    string Receivers,
                                    string Content,
                                    string Sender)
        {
            var rec = Receivers
                .Replace(" ", "")
                .Split(';');
            DatabaseOperations.SendMail(Title, Content, DateTime.Now, Sender, rec);
            return RedirectToAction("ListMessages", "Mailbox", new { mtype = "Incoming" });
        }
>>>>>>> Stashed changes
    }
}