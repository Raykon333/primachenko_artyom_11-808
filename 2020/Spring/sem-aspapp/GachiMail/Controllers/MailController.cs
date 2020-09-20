using System;
using Microsoft.AspNetCore.Mvc;
using GachiMail.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace GachiMail.Controllers
{
    [Authorize]
    public class MailController : Controller
    {
        IDatabaseService db;

        public MailController(IDatabaseService database)
        {
            db = database;
        }

        [HttpGet]
        public IActionResult Read(int id)
        {
            if (!db.MailboxContains(HttpContext.Session.GetString("Box"), id))
                return RedirectToAction("Folder", "Mailbox");
            else
            {

                var mail = db.GetMailPreview(id);
                var mailContent = db.GetMailContent(id);
                string[] addresses;
                if (mail.Sender == HttpContext.Session.GetString("Box"))
                    addresses = db.GetMailReceivers(id);
                else
                    addresses = new string[] { mail.Sender };
                var model = new ReadMailModel(
                    mail.Title,
                    mailContent,
                    mail.SendingTime,
                    addresses
                    );
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public IActionResult New(NewMailModel newMail)
        {
            var rec = newMail.Receivers
                .Replace(" ", "")
                .Replace("@gachimail.com","")
                .Split(';');
            if (rec.Any(r => !db.DoesMailboxExist(r)))
                ModelState.AddModelError("Receivers", "Неверно указаны получатели");
            if (ModelState.IsValid)
            {
                db.SendMail(newMail.Title, newMail.Content, DateTime.Now, HttpContext.Session.GetString("Box"), rec);
                return RedirectToAction("Folder", "Mailbox", 1);
            }
            return View(newMail);
        }
    }
}