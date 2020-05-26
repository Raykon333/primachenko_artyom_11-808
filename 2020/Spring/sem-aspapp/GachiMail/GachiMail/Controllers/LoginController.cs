using MailDatabase;
using GachiMail.Models;
using Microsoft.AspNetCore.Mvc;
using GachiMail.Utilities.Encoder;
namespace GachiMail.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Go(User user)
        {
            if (DatabaseOperations.PasswordCheck(user.Login, user.Password))
            {
                HttpContext
                    .Session
                    .Set("LI", ByteToASCIIEncoder.WriteToBytes(user.Login));
                return RedirectToAction("Incoming", "Mailbox");
            }
            else
                return RedirectToAction("Index", "Login");
        }
    }
}