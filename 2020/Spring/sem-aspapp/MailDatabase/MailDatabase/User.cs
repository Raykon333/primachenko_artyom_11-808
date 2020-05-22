using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase
{
    class User
    {
        [Column(Order = 0), MaxLength(20)]
        public string Login { get; internal set; }

        public string PasswordHash { get; internal set; }

        public User() { }
        public User(string login, string passwordHash)
        {
            Login = login;
            PasswordHash = passwordHash;
        }
    }
}
