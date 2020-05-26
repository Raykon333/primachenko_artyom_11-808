using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase
{
    class User
    {
        [Column(Order = 0), MaxLength(20)]
        public string UserLogin { get; internal set; }

        public string PasswordHash { get; internal set; }

        public int CurrencyAmount { get; internal set; }

        public byte TierLevel { get; internal set; }

        public User() { }
        public User(string login, string passwordHash)
        {
            UserLogin = login;
            PasswordHash = passwordHash;
            CurrencyAmount = 0;
            TierLevel = 0;
        }
    }
}
