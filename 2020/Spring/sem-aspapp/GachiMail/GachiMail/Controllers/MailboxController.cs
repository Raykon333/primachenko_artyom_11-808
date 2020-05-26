using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GachiMail.Utilities;
using GachiMail.Utilities.Encoder;
using MailDatabase;
using GachiMail.Models;
using MailDatabase.LetterTypes;
namespace GachiMail.Controllers
{
    public class MailboxController : MailAccountController
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Incoming()
        {
            byte[] box;
            HttpContext.Session.TryGetValue("Box", out box);
            ViewData["IncomingMessages"] =
                DatabaseOperations
                .GetMailIdsFromFolder<IncomingLetters>(ByteToASCIIEncoder.ReadFromBytes(box))
                .Select(a => new LetterPreview(a));
            return View();
        }
        public IActionResult Sent()
        {
            return View();
        }
    }
}