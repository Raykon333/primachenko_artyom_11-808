using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MailDatabase;
using MailDatabase.Exceptions;
using MailDatabase.LetterTypes;
<<<<<<< Updated upstream
using System;
using System.Reflection;
=======
using GachiMail.Utilities;
using GachiMail.Models;

>>>>>>> Stashed changes
namespace GachiMail.Controllers
{
    public class MailboxController : MailAccountController
    {
        private byte[] user
        {
            get
            {
                byte[] res;
                HttpContext.Session.TryGetValue("User", out res);
                return res;
            }
        }
        private byte[] box
        {
            get
            {
                byte[] res;
                HttpContext.Session.TryGetValue("Box", out res);
                if (res == null)
                    res = ByteToASCIIEncoder
                        .WriteToBytes(DatabaseOperations
                        .GetMailboxesByUser(ByteToASCIIEncoder
                        .ReadFromBytes(user))
                        .FirstOrDefault());
                return res;
            }
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListMessages(string mtype)
        {
            return (IActionResult)GetType().GetMethod(mtype).Invoke(this, null);
        }
        [NonAction]
        public IActionResult Incoming()
        {
            var links = DatabaseOperations
               .GetMailIdsFromFolder<IncomingLetters>(ByteToASCIIEncoder.ReadFromBytes(box))
               .Select(a => new LetterPreview(a))
               .ToList();
            ViewData["MessageType"] = "Incoming";
            return View("ListMessages", links);
        }
        [NonAction]
        public IActionResult Sent()
        {
            var links = DatabaseOperations
                .GetMailIdsFromFolder<SentLetters>(ByteToASCIIEncoder.ReadFromBytes(box))
                .Select(a => new LetterPreview(a))
                .ToList();
            ViewData["MessageType"] = "Sent";
            return View("ListMessages", links);
        }
<<<<<<< Updated upstream
=======

        public IActionResult MailboxCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult MBGo(string mailbox, string user)
        {
            try
            {
                DatabaseOperations.AddMailbox(user, mailbox);
            }
            catch (Exception ex)
            {
                if (ex is DatabaseException)
                    return RedirectToAction("MailboxCreate", "Mailbox");
            }
            return RedirectToAction("ProceedToMailbox", "Mailbox");
        }
>>>>>>> Stashed changes
    }
}