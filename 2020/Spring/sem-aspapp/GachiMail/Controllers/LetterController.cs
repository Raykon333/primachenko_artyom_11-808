using System;
using GachiMail.Utilities;
using Microsoft.AspNetCore.Mvc;
using GachiMail.Models;
using MailDatabase;
using MailDatabase.Exceptions;
using Microsoft.AspNetCore.Http;
namespace GachiMail.Controllers
{
    public class LetterController : MailAccountController
    {
        public IActionResult Index(int id)
        {
            try
            {
                return View(new Letter(id));
            }
            catch(Exception ex)
            {
                if (ex is DatabaseException)
                    return RedirectToAction("ListMessages", "Mailbox", new { mtype = "Incoming" });
            }
            return Ok();
        }
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
                .Replace("@gachimail.com","")
                .Split(';');
            DatabaseOperations.SendMail(Title, Content, DateTime.Now, Sender, rec);
            return RedirectToAction("ListMessages", "Mailbox", new { mtype = "Sent" });
        }
    }
}