using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase.Models
{
    public class User
    {
        [Column(Order = 0), MaxLength(20)]
        public string UserLogin { get; internal set; }

        public string PasswordHash { get; internal set; }

        public int CurrencyAmount { get; internal set; }

        public byte Tier { get; internal set; }

        public DateTime TierEndDate { get; internal set; }

        public User() { }
        public User(string userLogin, string passwordHash)
        {
            UserLogin = userLogin;
            PasswordHash = passwordHash;
            CurrencyAmount = 0;
            Tier = 0;
        }
    }
}
