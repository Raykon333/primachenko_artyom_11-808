using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GachiMail.Models
{
    public class ProfileModel
    {
        public string Login { get; set; }
        public int Tier { get; set; }
        public TimeSpan Expiration { get; set; }
        public int Currency { get; set; }
        public string[] Mailboxes { get; set; }

        public ProfileModel(string login, int tier, TimeSpan expiration, int currency, string[] mailboxes)
        {
            Login = login;
            Tier = tier;
            Expiration = expiration;
            Currency = currency;
            Mailboxes = mailboxes;
        }

        public ProfileModel() { }
    }
}
