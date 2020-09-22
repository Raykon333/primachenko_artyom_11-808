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

        public IActionResult Index()
        {
            return RedirectToAction("Profile", "Account");
        }
        public IActionResult ProceedToMailbox(string mailbox)
        {
            if (!db.GetMailboxesByUser(User.Identity.Name).Contains(mailbox))
            {
                return RedirectToAction("Profile", "Account");
            }
            HttpContext.Session.SetString("Box", mailbox);
            return RedirectToAction("Folder", 1);
        }

        [HttpPost]
        public IActionResult ChangeMailboxOptions(MailboxOptionsModel model)
        {
            return RedirectToAction("Folder", new
            {
                model.folder,
                model.pageSize,
                model.address
            });
        }

        public IActionResult Folder(int folder = 1, string sorting = "timeDesc", int pageSize = 10, int page = 1, string address = null)
        {
            if (pageSize < 1)
                pageSize = 1;
            if (page < 1)
                page = 1;
            sorting = sorting.ToLower();

            string cacheString = HttpContext.Session.GetString("Box") + "folder" + folder;

            List<MailPreview> allMailsPreviews;
            if (!cache.TryGetValue(cacheString, out allMailsPreviews) || db.FolderNeedsUpdating(HttpContext.Session.GetString("Box"), folder))
            {
                var allMailsIds = db.GetMailIdsFromFolder(HttpContext.Session.GetString("Box"), folder);
                allMailsPreviews = allMailsIds.Select(id => db.GetMailPreview(id)).ToList();
            }
            IEnumerable<MailPreview> result = allMailsPreviews;

            cache.Set(cacheString, allMailsPreviews, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            switch(sorting.ToLower())
            {
                case "time":
                    result = result.OrderBy(p => p.SendingTime);
                    break;
                case "timedesc":
                    result = result.OrderByDescending(p => p.SendingTime);
                    break;
                case "title":
                    result = result.OrderBy(p => p.Title);
                    break;
                case "titledesc":
                    result = result.OrderByDescending(p => p.Title);
                    break;
                case "content":
                    result = result.OrderBy(p => p.ContentPreview);
                    break;
                case "contentdesc":
                    result = result.OrderByDescending(p => p.ContentPreview);
                    break;
                case "address":
                    {
                        if (folder == 1)
                            result = result.OrderBy(p => p.Sender);
                        else if (folder == 2)
                            result = result.OrderBy(p => p.Receivers.First());
                        break;
                    }
                case "addressdesc":
                    {
                        if (folder == 1)
                            result = result.OrderByDescending(p => p.Sender);
                        else if (folder == 2)
                            result = result.OrderByDescending(p => p.Receivers.First());
                        break;
                    }
            }

            if (address != null)
            {
                if (folder == 1)
                    result = result.Where(p => p.Sender == address);
                else if (folder == 2)
                    result = result.Where(p => p.Receivers.Contains(address));
            }

            result = result
                .Skip(pageSize * (page - 1))
                .Take(pageSize);

            ViewBag.Page = page;
            ViewBag.LastPage = (allMailsPreviews.Count() - 1) / pageSize + 1;
            ViewBag.Folder = folder;
            ViewBag.Sorting = sorting;
            ViewBag.PageSize = pageSize;
            ViewBag.Address = address;

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