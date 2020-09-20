using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GachiMail.Utilities;
using MailDatabase;
using GachiMail.Models;
using MailDatabase.LetterTypes;
using Microsoft.AspNetCore.Http;
using System;
using System.Reflection;
using MailDatabase.Exceptions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace GachiMail.Controllers
{
    [Authorize]
    public class MailboxController : Controller
    {
        IMemoryCache cache;
        IDatabaseService db;

        public MailboxController(IDatabaseService database, IMemoryCache _cache)
        {
            db = database;
            cache = _cache;
        }

        private string user
        {
            get
            {
                return User.Identity.Name;
            }
        }
        private string box
        {
            get
            {
                   return db
                        .GetMailboxesByUser(user)
                        .FirstOrDefault();
            }
        }
        public IActionResult Index()
        {
            return RedirectToAction("Profile", "Account");
        }
        public IActionResult ProceedToMailbox(string mailbox)
        {
            if (box == null)
                return RedirectToAction("NewMailbox", "Account");
            if (mailbox == null)
                mailbox = box;
            HttpContext.Session.SetString("Box", mailbox);
            return RedirectToAction("Folder", 0);
        }

        [HttpPost]
        public IActionResult ChangeMailboxOptions(MailboxOptionsModel model)
        {
            return RedirectToAction("Folder", new
            {
                model.folder,
                model.sorting,
                model.pageSize,
                model.address
            });
        }

        public IActionResult Folder(int folder = 0, string sorting = "new", int pageSize = 10, int page = 1, string address = null)
        {
            if (pageSize < 1)
                pageSize = 1;
            if (page < 1)
                page = 1;
            sorting = sorting.ToLower();

            List<MailPreview> allMailsPreviews;
            if (!cache.TryGetValue("folder" + folder, out allMailsPreviews))
            {
                var allMailsIds = db.GetMailIdsFromFolder(HttpContext.Session.GetString("Box"), folder);
                allMailsPreviews = allMailsIds.Select(id => db.GetMailPreview(id)).ToList();
            }
            IEnumerable<MailPreview> result = allMailsPreviews;

            cache.Set("folder" + folder, allMailsPreviews, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            if (sorting == "old")
                result = result.OrderBy(p => p.SendingTime);
            else if (sorting == "new")
                result = result.OrderByDescending(p => p.SendingTime);

            if (address != null)
            {
                if (folder == 0)
                    result = result.Where(p => p.Sender == address);
                else if (folder == 1)
                    result = result.Where(p => p.Receivers.Contains(address));
            }

            result = result
                .Skip(pageSize * (page - 1))
                .Take(pageSize);

            ViewBag.Page = page;
            ViewBag.LastPage = (allMailsPreviews.Count() - 1) / pageSize + 1;

            return View(result.ToList());
        }

        public IActionResult SwitchPage(int newPage, RouteValueDictionary pars)
        {
            return RedirectToAction("Folder", new
            {
                folder = pars["folder"],
                oldFirst = pars["oldFirst"],
                pageSize = pars["pageSize"],
                page = newPage,
                sender = pars["sender"]
            });
        }
    }
}