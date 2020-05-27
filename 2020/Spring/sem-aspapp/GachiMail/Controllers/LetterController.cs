using GachiMail.Utilities;
using Microsoft.AspNetCore.Mvc;
using GachiMail.Models;
namespace GachiMail.Controllers
{
    public class LetterController : MailAccountController
    {
        public IActionResult Index(int id)
        {
            ViewData["Mail"] = new Letter(id);
            return View();
        }
        public IActionResult WriteLetter()
        {
            return View();
        }
    }
}