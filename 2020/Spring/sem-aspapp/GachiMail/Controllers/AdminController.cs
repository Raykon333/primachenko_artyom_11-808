using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GachiMail.Controllers
{
    [Authorize(Roles="admin")]
    public class AdminController : Controller
    {
        string cs = "Host=localhost;Port=5432;Database=MailDb;Username=postgres;Password=postgres";

        public IActionResult Index()
        {
            return View(model: cs);
        }
    }
}
