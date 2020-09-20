using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase.Models
{
    public class User
    {
        [Column(Order = 0), MaxLength(20)]
        public string UserLogin { get; set; }
        public string PasswordHash { get; set; }
        public int CurrencyAmount { get; set; }
        public string Role { get; set; }
        public byte Tier { get; set; }
        public DateTime TierEndDate { get; set; }

        public User() { }
        public User(string userLogin, string passwordHash)
        {
            UserLogin = userLogin;
            PasswordHash = passwordHash;
            CurrencyAmount = 0;
            Tier = 0;
            Role = "user";
        }
    }
}
