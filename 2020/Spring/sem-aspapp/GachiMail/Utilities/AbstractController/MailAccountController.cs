using System;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using MailDatabase;
using GachiMail.Models;

namespace GachiMail.Utilities
{
    public abstract class MailAccountController : Controller
    {
        public override Task OnActionExecutionAsync
            (ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Session.Keys.Contains("LI"))
            {
                if (context
                    .HttpContext.Request
                    .Cookies.ContainsKey("LP"))
                {
                    var info = JsonSerializer.Deserialize<User>(context
                        .HttpContext
                        .Request
                        .Cookies["LP"]);
                    context.HttpContext.Session
                        .SetString("LI", DatabaseOperations
                            .PasswordCheck(info.Login, info.Password).ToString());
                    context.HttpContext.Session
                        .SetString("User", info.Login);
                }
                else
                {
                    context.HttpContext.Session
                        .SetString("LI", "false");
                    context.Result = RedirectToAction("Index", "Login");
                }
            }
            else
            {
                //Здесь, например, код для того, чтобы вызвать редирект
                if (context.HttpContext.Session.GetString("LI") == "false")
                    context.Result = RedirectToAction("Index", "Login");
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}