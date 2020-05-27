using MailDatabase;
using GachiMail.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using GachiMail.Utilities.Encoder;
using Microsoft.AspNetCore.Http;
namespace GachiMail.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Go(User user, bool savecookies)
        {
            if (DatabaseOperations.PasswordCheck(user.Login, user.Password))
            {
                HttpContext.Session.SetString("LI", "true");
                HttpContext.Session.SetString("User", user.Login);
                if(savecookies)
                    HttpContext.Response.Cookies
                        .Append("LP", JsonSerializer.Serialize<User>(user));
                return RedirectToAction("ListMessages", "Mailbox", new { mtype = "Incoming" });
            }
            else
                return RedirectToAction("Index", "Login");
        }
    }
}