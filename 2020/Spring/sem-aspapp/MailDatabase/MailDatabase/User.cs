using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MailDatabase
{
    class User
    {
        [Column(Order = 0)]
        //public string UserId { get; internal set; }

        [MaxLength(20)]
        public string Login { get; internal set; }

        public string PasswordHash { get; internal set; }

        public User() { }
        public User(/*string userId,*/ string login, string passwordHash)
        {
            //UserId = userId;
            Login = login;
            PasswordHash = passwordHash;
        }
    }
}
