using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GachiMail.Utilities;
using GachiMail.Utilities.Encoder;
using MailDatabase;
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
                .GetMailIdsFromFolder(ByteToASCIIEncoder.ReadFromBytes(box), 0);
            return View();
        }
        public IActionResult Sended()
        {
            return View();
        }
    }
}