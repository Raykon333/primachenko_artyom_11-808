using System.Linq;
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
            byte[] user;
            HttpContext.Session.TryGetValue("Box", out box);
            HttpContext.Session.TryGetValue("User", out user);
            string us = ByteToASCIIEncoder.ReadFromBytes(user);
            if (box == null)
                box = ByteToASCIIEncoder
                    .WriteToBytes(DatabaseOperations
                    .GetMailboxesByUser(ByteToASCIIEncoder
                    .ReadFromBytes(user))
                    .FirstOrDefault());
            var links = DatabaseOperations
                .GetMailIdsFromFolder<IncomingLetters>(ByteToASCIIEncoder.ReadFromBytes(box))
                .Select(a => new LetterPreview(a))
                .ToList();
            return View(links);
        }
        public IActionResult Sent()
        {
            return View();
        }
    }
}