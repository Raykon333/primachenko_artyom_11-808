using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MailDatabase;
using GachiMail.Models;
using System.Text;
using System.Linq;

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
                    context
                        .HttpContext
                        .Session
                        .Set("LI",
                        Encoding.ASCII.GetBytes(DatabaseOperations
                        .PasswordCheck(info.Login, info.Password).ToString()));
                }
                else
                {
                    context
                        .HttpContext
                        .Session
                        .Set("LI", Encoding.ASCII.GetBytes("false"));
                    context.Result = RedirectToAction("Index", "Login");
                }
            }
            else
            {
                //Здесь, например, код для того, чтобы вызвать редирект
                byte[] value;
                if (context.HttpContext.Session.TryGetValue("LI", out value) &&
                    Encoding.ASCII.GetString(value) == "false")
                    context.Result = RedirectToAction("Index", "Login");
                
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}