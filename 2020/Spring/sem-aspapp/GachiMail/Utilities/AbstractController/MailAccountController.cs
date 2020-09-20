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
using Microsoft.AspNetCore.Http;
namespace GachiMail.Utilities
{
    public abstract class MailAccountController : Controller
    {
        IDatabaseService db;

        public MailAccountController(IDatabaseService database)
        {
            db = database;
        }
    }
}